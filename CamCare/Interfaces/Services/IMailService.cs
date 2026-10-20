using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Versendet E-Mails als HTML über den konfigurierten SMTP-Server (MailKit).
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Versendet eine HTML-E-Mail an die angegebenen Empfänger. Es wird keine
        /// Ausnahme geworfen; Fehler werden im <see cref="ServiceResponse{TData}"/> gemeldet.
        /// </summary>
        Task<ServiceResponse<bool>> SendAsync(
            MailSettingsVm settings,
            IEnumerable<string> recipients,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default);
    }
}
