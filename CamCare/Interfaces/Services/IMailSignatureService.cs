using CamCare.Domain;
using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Verwaltet die E-Mail-Signatur, die an versendete E-Mails angehängt wird.
    /// </summary>
    public interface IMailSignatureService : IDbService<MailSignature, MailSignatureVm>
    {
        /// <summary>
        /// Liefert die zu bearbeitende Signatur. Ist noch keine Signatur vorhanden, wird eine
        /// neue, noch nicht gespeicherte Signatur (Id 0) mit Standardinhalt zurückgegeben.
        /// </summary>
        Task<ServiceResponse<MailSignatureVm>> GetSignatureAsync(CancellationToken cancellationToken = default);
    }
}
