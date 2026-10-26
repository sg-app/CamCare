using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    /// <summary>
    /// Die Signatur, die an jede über CamCare versendete E-Mail angehängt wird
    /// (automatischer Statusversand und Testversand). Sie besteht aus einem HTML-Text und
    /// einem optionalen Logo, das direkt in die E-Mail eingebettet wird.
    /// </summary>
    public class MailSignature : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Kennzeichnet, ob die Signatur an versendete E-Mails angehängt wird.</summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// HTML-Inhalt der Signatur. Der Platzhalter <c>{{Logo}}</c> markiert die Stelle,
        /// an der das Logo ausgegeben wird (<see cref="MailSettingsDefaults.SignatureLogoPlaceholder"/>).
        /// </summary>
        public string HtmlTemplate { get; set; } = string.Empty;

        /// <summary>Dateiname des Logos (Anzeige und Ermittlung des Inhaltstyps).</summary>
        [MaxLength(260)]
        public string? LogoFileName { get; set; }

        /// <summary>Das Logo als Bilddatei; wird als eingebettetes Bild (Content-Id) versendet.</summary>
        public byte[]? LogoData { get; set; }
    }
}
