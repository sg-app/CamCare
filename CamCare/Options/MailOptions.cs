namespace CamCare.Options
{
    /// <summary>
    /// Konfiguration des E-Mail-Versands (SMTP-Zugang). Die Werte werden aus dem Abschnitt
    /// <c>Mail</c> der <c>appsettings.json</c> bzw. aus Umgebungsvariablen
    /// (z. B. <c>Mail__SmtpHost</c>) gelesen und nicht in der Datenbank gespeichert.
    /// </summary>
    public class MailOptions
    {
        public const string SectionName = "Mail";

        // --- SMTP-Server ---

        /// <summary>Hostname oder IP-Adresse des SMTP-Servers.</summary>
        public string SmtpHost { get; set; } = string.Empty;

        /// <summary>Port des SMTP-Servers (z. B. 25, 465 oder 587).</summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>Verschlüsselung der SMTP-Verbindung.</summary>
        public MailSecurityMode Security { get; set; } = MailSecurityMode.StartTls;

        /// <summary>Benutzername für die SMTP-Authentifizierung (optional).</summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Passwort für die SMTP-Authentifizierung (optional). Sollte in Produktion über eine
        /// sichere Umgebungsvariable (<c>Mail__Password</c>) gesetzt werden.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>Zeitlimit für den kompletten Versandvorgang in Sekunden.</summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>Absenderadresse (From) der versendeten E-Mails.</summary>
        public string FromAddress { get; set; } = string.Empty;

        /// <summary>Anzeigename des Absenders (optional).</summary>
        public string? FromName { get; set; }

        // --- Automatischer Versand ---

        /// <summary>
        /// Hauptschalter für den E-Mail-Versand. Ist er deaktiviert, wird beim Statuswechsel
        /// keine E-Mail versendet (der Testversand bleibt möglich).
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gibt an, ob die Konfiguration für den Versand ausreicht (Server und Absenderadresse).
        /// </summary>
        public bool IsConfigured
            => !string.IsNullOrWhiteSpace(SmtpHost) && !string.IsNullOrWhiteSpace(FromAddress);
    }
}
