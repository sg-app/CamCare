using CamCare.Domain;

namespace CamCare.Models
{
    /// <summary>
    /// Die Signatur, die an versendete E-Mails angehängt wird. Der HTML-Inhalt kann den
    /// Platzhalter <see cref="MailSettingsDefaults.SignatureLogoPlaceholder"/> enthalten,
    /// an dessen Stelle das Logo eingefügt wird.
    /// </summary>
    public class MailSignatureVm
    {
        public int Id { get; set; }

        /// <summary>Kennzeichnet, ob die Signatur an versendete E-Mails angehängt wird.</summary>
        public bool IsEnabled { get; set; }

        public string HtmlTemplate { get; set; } = string.Empty;

        public string? LogoFileName { get; set; }

        public byte[]? LogoData { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        /// <summary>Kennzeichnet eine noch nicht gespeicherte Signatur.</summary>
        public bool IsNew => Id <= 0;

        /// <summary>Kennzeichnet, ob ein Logo hinterlegt ist.</summary>
        public bool HasLogo => LogoData is { Length: > 0 };

        /// <summary>Inhaltstyp des Logos (aus dem Dateinamen abgeleitet).</summary>
        public string LogoContentType => ImageContentTypes.FromFileName(LogoFileName);

        /// <summary>Das Logo als Data-URL – für die Vorschau in den Einstellungen.</summary>
        public string? LogoDataUrl => HasLogo
            ? $"data:{LogoContentType};base64,{Convert.ToBase64String(LogoData!)}"
            : null;

        /// <summary>Dateigröße des Logos als Text (z. B. „12 KB“).</summary>
        public string LogoSizeText => HasLogo
            ? $"{Math.Max(1d, LogoData!.Length / 1024d):F0} KB"
            : "kein Logo";

        public override string ToString() => $"E-Mail-Signatur {Id}";
    }
}
