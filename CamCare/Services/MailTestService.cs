using CamCare.Interfaces.Services;
using CamCare.Models;
using CamCare.Models.Amicron;

namespace CamCare.Services
{
    /// <summary>
    /// Führt den Testversand der E-Mail-Vorlagen mit der in der <c>appsettings.json</c>
    /// konfigurierten SMTP-Konfiguration aus.
    /// </summary>
    public class MailTestService(
        ILogger<MailTestService> logger,
        IMailTemplateRenderer templateRenderer,
        IMailService mailService,
        IRepairOrderService repairOrderService) : IMailTestService
    {
        public async Task<ServiceResponse<bool>> SendTestMailAsync(MailTemplateVm? template, string recipient, int? repairOrderId)
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

                return await mailService.SendAsync([recipient], subject, body);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Testversand der E-Mail an {Recipient}", recipient);
                return ServiceResponse.Failure<bool>($"Fehler beim Testversand: {ex.Message}", ex);
            }
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
    }
}
