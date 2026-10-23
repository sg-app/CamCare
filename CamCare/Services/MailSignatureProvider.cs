using System.Text.RegularExpressions;
using CamCare.Domain;
using CamCare.Interfaces.Persistence;
using CamCare.Interfaces.Services;
using CamCare.Models;
using Microsoft.EntityFrameworkCore;

namespace CamCare.Services
{
    /// <summary>
    /// Lädt die aktive E-Mail-Signatur und bereitet sie für den Versand vor: Der Logo-Platzhalter
    /// (<see cref="MailSettingsDefaults.SignatureLogoPlaceholder"/>) wird durch ein eingebettetes
    /// Bild (<c>cid:</c>) ersetzt, damit das Logo ohne externen Aufruf angezeigt wird.
    /// </summary>
    public class MailSignatureProvider(
        IAppDbContextFactory contextFactory,
        ILogger<MailSignatureProvider> logger) : IMailSignatureProvider
    {
        /// <summary>Content-Id, unter der das Logo in der HTML-E-Mail eingebettet wird.</summary>
        public const string LogoContentId = "camcare-signature-logo";

        /// <summary>Stil des Logos in der E-Mail (Inline-Styles, da E-Mail-Clients kein CSS laden).</summary>
        private const string LogoImageStyle = "max-width:220px;height:auto;border:0;";

        public async Task<MailSignatureContent?> GetActiveSignatureAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var context = contextFactory.CreateDbContext();

                var signature = await context.MailSignatures
                    .AsNoTracking()
                    .Where(s => s.IsEnabled && !s.ArchivedAt.HasValue)
                    .OrderBy(s => s.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (signature is null || string.IsNullOrWhiteSpace(signature.HtmlTemplate))
                    return null;

                var logo = CreateLogo(signature);

                return new MailSignatureContent
                {
                    Html = ReplaceLogoPlaceholder(signature.HtmlTemplate, logo),
                    Logo = logo
                };
            }
            catch (Exception ex)
            {
                // Ein Fehler in der Signatur darf den E-Mail-Versand nicht verhindern.
                logger.LogError(ex, "Die E-Mail-Signatur konnte nicht geladen werden.");
                return null;
            }
        }

        /// <summary>Baut das einzubettende Logo aus der Signatur (oder <c>null</c>, wenn keines hinterlegt ist).</summary>
        private static MailInlineImage? CreateLogo(MailSignature signature)
        {
            if (signature.LogoData is not { Length: > 0 })
                return null;

            var fileName = string.IsNullOrWhiteSpace(signature.LogoFileName)
                ? "logo.png"
                : signature.LogoFileName.Trim();

            return new MailInlineImage
            {
                ContentId = LogoContentId,
                FileName = fileName,
                ContentType = ImageContentTypes.FromFileName(fileName),
                Data = signature.LogoData
            };
        }

        /// <summary>
        /// Ersetzt den Logo-Platzhalter durch das eingebettete Bild. Enthält die Signatur keinen
        /// Platzhalter, wird das Logo am Ende angehängt; ohne Logo wird der Platzhalter entfernt.
        /// </summary>
        private static string ReplaceLogoPlaceholder(string template, MailInlineImage? logo)
        {
            var logoHtml = logo is null
                ? string.Empty
                : $"<img src=\"cid:{logo.ContentId}\" alt=\"Logo\" style=\"{LogoImageStyle}\" />";

            var placeholder = MailSettingsDefaults.SignatureLogoPlaceholder;

            if (!template.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
                return logo is null ? template : template + logoHtml;

            return Regex.Replace(
                template,
                Regex.Escape(placeholder),
                _ => logoHtml,
                RegexOptions.IgnoreCase);
        }
    }
}
