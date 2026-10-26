using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Führt den Testversand von E-Mail-Vorlagen mit der in der <c>appsettings.json</c>
    /// konfigurierten SMTP-Konfiguration (<c>Mail</c>-Abschnitt) aus.
    /// </summary>
    public interface IMailTestService
    {
        /// <summary>
        /// Versendet die übergebene Vorlage testweise an eine manuell eingegebene
        /// Adresse. Ist <paramref name="repairOrderId"/> gesetzt, werden die Daten dieses
        /// Auftrags in die Vorlage eingesetzt, ansonsten Beispieldaten. Ohne Vorlage wird
        /// eine einfache Test-E-Mail versendet (Prüfung der SMTP-Einstellungen).
        /// </summary>
        Task<ServiceResponse<bool>> SendTestMailAsync(MailTemplateVm? template, string recipient, int? repairOrderId);
    }
}
