using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using CamCare.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CamCare.Services
{
    /// <summary>
    /// Versendet die für einen Status konfigurierten E-Mail-Vorlagen für einen Reparaturauftrag.
    /// <para>
    /// Es können beliebig viele Vorlagen konfiguriert werden (z. B. eine für den Status
    /// "Eingetroffen" und eine für "Versendet"). Für jede Vorlage gilt: Sie wird nur versendet,
    /// wenn sie aktiv ist und für diesen Auftrag und diese Vorlage noch keine E-Mail erfolgreich
    /// versendet wurde. Dadurch wird ein versehentlicher Mehrfachversand bei mehrfacher
    /// Statusänderung verhindert. Fehlgeschlagene Versuche werden protokolliert, blockieren
    /// aber keinen erneuten Versuch bei einer späteren Statusänderung.
    /// </para>
    /// </summary>
    public class RepairOrderMailDispatcher(
        IAppDbContextFactory contextFactory,
        IMapper mapper,
        IMailTemplateRenderer templateRenderer,
        IMailService mailService,
        IMailSignatureProvider signatureProvider,
        IAmicronDataService amicronDataService,
        IOptions<MailOptions> mailOptions,
        ILogger<RepairOrderMailDispatcher> logger) : IRepairOrderMailDispatcher
    {
        private readonly MailOptions _mailOptions = mailOptions.Value;

        public async Task DispatchAsync(int repairOrderId, int repairOrderStatusId)
        {
            try
            {
                // Hauptschalter (appsettings.json: Mail:IsEnabled). Ist er deaktiviert, wird
                // kein automatischer Statusversand ausgeführt.
                if (!_mailOptions.IsEnabled)
                {
                    logger.LogDebug("E-Mail-Versand ist deaktiviert (Auftrag {RepairOrderId}).", repairOrderId);
                    return;
                }

                using var context = contextFactory.CreateDbContext();

                var templates = await context.MailTemplates
                    .AsNoTracking()
                    .Include(t => t.RepairOrderStatus)
                    .Where(t => t.RepairOrderStatusId == repairOrderStatusId && t.IsEnabled)
                    .OrderBy(t => t.Order)
                    .ThenBy(t => t.Id)
                    .ToListAsync();

                if (templates.Count == 0)
                {
                    logger.LogDebug(
                        "Für Status {StatusId} ist keine aktive E-Mail-Vorlage konfiguriert (Auftrag {RepairOrderId}).",
                        repairOrderStatusId, repairOrderId);
                    return;
                }

                var entity = await context.RepairOrders
                    .AsNoTracking()
                    .Include(r => r.RepairOrderStatus)
                    .Include(r => r.EmailAddresses)
                    .FirstOrDefaultAsync(r => r.Id == repairOrderId);

                if (entity is null)
                    return;

                var orderVm = mapper.Map<RepairOrder, RepairOrderVm>(entity);

                var recipients = (orderVm.EmailAddresses ?? [])
                    .Select(a => a.Address)
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (recipients.Count == 0)
                {
                    logger.LogWarning(
                        "Für Auftrag {RepairOrderId} sind keine E-Mail-Adressen hinterlegt – es wurde keine Status-E-Mail versendet.",
                        repairOrderId);
                    return;
                }

                // Kundenanschrift (Amicron) ergänzen, damit die Kunden-Platzhalter gefüllt sind.
                await ApplyCustomerAsync(orderVm);

                foreach (var template in templates)
                {
                    await SendTemplateAsync(context, template, orderVm, recipients, repairOrderId, repairOrderStatusId);
                }
            }
            catch (Exception ex)
            {
                // Ein Mailfehler darf das Speichern des Reparaturauftrags niemals beeinträchtigen.
                logger.LogError(ex, "Fehler beim automatischen Versand der Status-E-Mail für Auftrag {RepairOrderId}.", repairOrderId);
            }
        }
        /// <summary>
        /// Versendet eine einzelne Vorlage. Fehler einer Vorlage verhindern nicht den
        /// Versand der übrigen Vorlagen.
        /// </summary>
        private async Task SendTemplateAsync(
            IAppDbContext context,
            MailTemplate template,
            RepairOrderVm orderVm,
            IReadOnlyList<string> recipients,
            int repairOrderId,
            int repairOrderStatusId)
        {
            try
            {
                // Mehrfachversand verhindern: Für diesen Auftrag und diese Vorlage darf
                // höchstens einmal erfolgreich versendet worden sein.
                var alreadySent = await context.RepairOrderMailLogs
                    .AsNoTracking()
                    .AnyAsync(l => l.RepairOrderId == repairOrderId
                                   && l.MailTemplateId == template.Id
                                   && l.IsSuccess);

                if (alreadySent)
                {
                    logger.LogInformation(
                        "E-Mail-Vorlage {MailTemplateId} wurde für Auftrag {RepairOrderId} bereits versendet – kein erneuter Versand.",
                        template.Id, repairOrderId);
                    return;
                }

                var subject = templateRenderer.Render(template.SubjectTemplate, orderVm);
                var body = templateRenderer.Render(template.BodyTemplate, orderVm);

                // Die konfigurierte Signatur (inkl. Logo) wird an jede E-Mail angehängt.
                var (bodyWithSignature, inlineImages) = await BuildBodyWithSignatureAsync(body);

                var result = await mailService.SendAsync(recipients, subject, bodyWithSignature, inlineImages);

                context.RepairOrderMailLogs.Add(new RepairOrderMailLog
                {
                    RepairOrderId = repairOrderId,
                    MailTemplateId = template.Id,
                    RepairOrderStatusId = repairOrderStatusId,
                    StatusName = template.RepairOrderStatus?.Name,
                    Recipients = string.Join(", ", recipients),
                    Subject = subject,
                    IsSuccess = result.Success,
                    ErrorMessage = result.Success ? null : Truncate(result.Message, 2000),
                    SentAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync();

                if (!result.Success)
                {
                    logger.LogWarning(
                        "E-Mail-Vorlage {MailTemplateId} für Auftrag {RepairOrderId} konnte nicht versendet werden: {Message}",
                        template.Id, repairOrderId, result.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Fehler beim automatischen Versand der E-Mail-Vorlage {MailTemplateId} für Auftrag {RepairOrderId}.",
                    template.Id, repairOrderId);
            }
        }

        /// <summary>
        /// Hängt die aktive E-Mail-Signatur an den Inhalt an und liefert das einzubettende Logo
        /// (falls vorhanden) für den Versand mit.
        /// </summary>
        private async Task<(string Body, List<MailInlineImage> InlineImages)> BuildBodyWithSignatureAsync(string body)
        {
            var signature = await signatureProvider.GetActiveSignatureAsync();

            if (signature is null)
                return (body, []);

            var inlineImages = new List<MailInlineImage>();
            if (signature.Logo is not null)
                inlineImages.Add(signature.Logo);

            return (MailBodyComposer.AppendSignature(body, signature.Html), inlineImages);
        }

        private async Task ApplyCustomerAsync(RepairOrderVm orderVm)
        {
            if (orderVm.CustomerId is null || orderVm.Customer is not null)
                return;

            var response = await amicronDataService.GetAddressByCustomerIdAsync(orderVm.CustomerId.Value);
            if (response.Success && response.Data is not null)
                orderVm.Customer = response.Data;
        }

        private static string? Truncate(string? value, int maxLength)
            => value is null || value.Length <= maxLength ? value : value[..maxLength];


    }
}
