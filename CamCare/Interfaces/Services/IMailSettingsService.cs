using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Liest und speichert die Konfiguration des E-Mail-Versands und ermöglicht einen Testversand.
    /// </summary>
    public interface IMailSettingsService
    {
        /// <summary>Liefert die (einzige) Konfiguration; legt sie bei Bedarf neu an.</summary>
        Task<ServiceResponse<MailSettingsVm>> GetAsync();

        /// <summary>Speichert die Konfiguration.</summary>
        Task<ServiceResponse<MailSettingsVm>> SaveAsync(MailSettingsVm vm);

        /// <summary>
        /// Versendet die übergebene Vorlage testweise an eine manuell eingegebene
        /// Adresse. Ist <paramref name="repairOrderId"/> gesetzt, werden die Daten dieses
        /// Auftrags in die Vorlage eingesetzt, ansonsten Beispieldaten. Ohne Vorlage wird
        /// eine einfache Test-E-Mail versendet (Prüfung der SMTP-Einstellungen).
        /// </summary>
        Task<ServiceResponse<bool>> SendTestMailAsync(MailSettingsVm vm, MailTemplateVm? template, string recipient, int? repairOrderId);
    }
}
