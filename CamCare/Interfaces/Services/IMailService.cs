using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Versendet E-Mails als HTML über den in der <c>appsettings.json</c> konfigurierten
    /// SMTP-Server (MailKit).
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Versendet eine HTML-E-Mail an die angegebenen Empfänger. Die SMTP-Konfiguration
        /// wird dabei aus dem <c>Mail</c>-Abschnitt der <c>appsettings.json</c> gelesen.
        /// Es wird keine Ausnahme geworfen; Fehler werden im <see cref="ServiceResponse{TData}"/>
        /// gemeldet.
        /// </summary>
        Task<ServiceResponse<bool>> SendAsync(
            IEnumerable<string> recipients,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default);
    }
}
