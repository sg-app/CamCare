using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using CamCare.Models.Amicron;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace CamCare.Services
{
    /// <summary>
    /// Liest und speichert die (einzige) E-Mail-Konfiguration und führt den Testversand aus.
    /// </summary>
    public class MailSettingsService(
        IAppDbContextFactory contextFactory,
        IMapper mapper,
        ILogger<MailSettingsService> logger,
        NotificationService notificationService,
        IMailTemplateRenderer templateRenderer,
        IMailService mailService,
        IRepairOrderService repairOrderService) : IMailSettingsService
    {
        public async Task<ServiceResponse<MailSettingsVm>> GetAsync()
        {
            try
            {
                using var context = contextFactory.CreateDbContext();
                var entity = await LoadOrCreateAsync(context);

                return ServiceResponse.Success(mapper.Map<MailSettings, MailSettingsVm>(entity));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Laden der E-Mail-Konfiguration");
                NotifyError("Fehler beim Laden der E-Mail-Konfiguration");
                return ServiceResponse.Failure<MailSettingsVm>("Fehler beim Laden der E-Mail-Konfiguration", ex);
            }
        }

        public async Task<ServiceResponse<MailSettingsVm>> SaveAsync(MailSettingsVm vm)
        {
            try
            {
                using var context = contextFactory.CreateDbContext();

                var entity = await context.MailSettings
                    .OrderBy(s => s.Id)
                    .FirstOrDefaultAsync();

                if (entity is null)
                {
                    entity = new MailSettings { CreatedAt = DateTime.UtcNow };
                    context.MailSettings.Add(entity);
                }

                vm.Id = entity.Id;
                mapper.Map(vm, entity);
                entity.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                var reloaded = await context.MailSettings
                    .AsNoTracking()
                    .FirstAsync(s => s.Id == entity.Id);

                return ServiceResponse.Success(mapper.Map<MailSettings, MailSettingsVm>(reloaded));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Speichern der E-Mail-Konfiguration");
                NotifyError("Fehler beim Speichern der E-Mail-Konfiguration");
                return ServiceResponse.Failure<MailSettingsVm>("Fehler beim Speichern der E-Mail-Konfiguration", ex);
            }
        }

        public async Task<ServiceResponse<bool>> SendTestMailAsync(MailSettingsVm vm, MailTemplateVm? template, string recipient, int? repairOrderId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(recipient))
                    return ServiceResponse.Failure<bool>("Bitte eine Empfängeradresse für den Testversand angeben.");

                // Wurde ein Auftrag ausgewählt, werden dessen Daten eingesetzt, ansonsten Beispieldaten.
                RepairOrderVm? order = null;
                if (repairOrderId.HasValue)
                {
                    var orderResponse = await repairOrderService.GetByIdAsync(repairOrderId.Value);
                    if (orderResponse.Success)
                        order = orderResponse.Data;
                }

                order ??= CreateSampleOrder();

                // Ohne ausgewählte Vorlage wird eine einfache Test-E-Mail versendet, damit
                // auch die SMTP-Einstellungen allein geprüft werden können.
                var subject = string.IsNullOrWhiteSpace(template?.SubjectTemplate)
                    ? "Testmail CamCare"
                    : templateRenderer.Render(template.SubjectTemplate, order);

                var body = string.IsNullOrWhiteSpace(template?.BodyTemplate)
                    ? "<p>Testmail von CamCare – die SMTP-Einstellungen funktionieren.</p>"
                    : templateRenderer.Render(template.BodyTemplate, order);

                return await mailService.SendAsync(vm, [recipient], subject, body);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Testversand der E-Mail an {Recipient}", recipient);
                return ServiceResponse.Failure<bool>($"Fehler beim Testversand: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lädt die Konfiguration. Ist noch keine vorhanden (z. B. weil der Datensatz
        /// gelöscht wurde), wird sie mit Standardwerten neu angelegt.
        /// </summary>
        private static async Task<MailSettings> LoadOrCreateAsync(IAppDbContext context)
        {
            var entity = await context.MailSettings
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .FirstOrDefaultAsync();

            if (entity is not null)
                return entity;

            entity = new MailSettings
            {
                SmtpPort = 587,
                Security = MailSecurityMode.StartTls,
                TimeoutSeconds = 30,
                CreatedAt = DateTime.UtcNow
            };

            context.MailSettings.Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Beispielauftrag für den Testversand, wenn kein echter Auftrag ausgewählt wurde.
        /// </summary>
        private static RepairOrderVm CreateSampleOrder() => new()
        {
            Id = 42,
            CustomerId = 10042,
            IsCamera = true,
            OrderNumber = "A-2026-00042",
            QuoteNumber = "AN-2026-00042",
            DeliveryNoteNumber = "LS-2026-00042",
            CameraSerialNumber = "9876543210",
            RepairOrderStatus = new RepairOrderStatusVm { Id = 9, Name = "Versendet" },
            Customer = new Adressen
            {
                LfdNr = 10042,
                KdNummer = "10042",
                Vorname = "Max",
                Name = "Mustermann",
                Strasse = "Musterstraße 1",
                Plz = "12345",
                Ort = "Musterstadt"
            },
            DeliveryAddress = new DeliveryAddressVm
            {
                Recipient = "Max Mustermann",
                Street = "Musterstraße 1",
                PostalCode = "12345",
                City = "Musterstadt"
            },
            Defectives = [new DefectiveVm { Description = "Beispieldefekt" }],
            Works = [new WorkVm { Description = "Beispielarbeit" }],
            IncludedComponents = [new IncludedComponentVm { Description = "Beispielkomponente" }],
            RepairPositions = [new RepairPositionVm { Description = "Beispielmaterial", Quantity = 1 }],
            Employees = [new EmployeeVm { FirstName = "Erika", LastName = "Musterfrau" }],
            PackagingComment = "Kundenkarton",
            EmailAddresses = [new EmailAddressVm { CustomerId = 10042, Address = "kunde@example.com" }],
            CreatedAt = DateTime.UtcNow,
            CurrentStateAt = DateTime.UtcNow,
            ArrivedAt = DateTime.UtcNow
        };

        private void NotifyError(string summary, string? message = null)
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = summary,
                Detail = message ?? string.Empty,
                Duration = 5000
            });
        }
    }
}
