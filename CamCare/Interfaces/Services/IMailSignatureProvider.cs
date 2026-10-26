using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Liefert die aktive E-Mail-Signatur (HTML inkl. eingebettetem Logo) für den Versand.
    /// </summary>
    public interface IMailSignatureProvider
    {
        /// <summary>
        /// Liefert die aktive Signatur oder <c>null</c>, wenn keine Signatur aktiviert ist.
        /// Fehler werden nicht geworfen, sondern protokolliert – ein Problem mit der Signatur
        /// darf den E-Mail-Versand nicht verhindern.
        /// </summary>
        Task<MailSignatureContent?> GetActiveSignatureAsync(CancellationToken cancellationToken = default);
    }
}
