using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    /// <summary>
    /// Konfiguration des E-Mail-Versands: SMTP-Zugang sowie die Vorlage, die beim
    /// Erreichen eines konfigurierbaren Status automatisch an die E-Mail-Adressen
    /// des Reparaturauftrags versendet wird.
    /// Es existiert genau ein Konfigurationsdatensatz (<see cref="Id"/> = 1).
    /// </summary>
    public class MailSettings : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        // --- SMTP-Server ---

        /// <summary>Hostname oder IP-Adresse des SMTP-Servers.</summary>
        [MaxLength(255)]
        public string SmtpHost { get; set; } = string.Empty;

        /// <summary>Port des SMTP-Servers (z. B. 25, 465 oder 587).</summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>Verschlüsselung der SMTP-Verbindung.</summary>
        public MailSecurityMode Security { get; set; } = MailSecurityMode.StartTls;

        /// <summary>Benutzername für die SMTP-Authentifizierung (optional).</summary>
        [MaxLength(255)]
        public string? UserName { get; set; }

        /// <summary>Passwort für die SMTP-Authentifizierung (optional).</summary>
        [MaxLength(500)]
        public string? Password { get; set; }

        /// <summary>Zeitlimit für den kompletten Versandvorgang in Sekunden.</summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>Absenderadresse (From) der versendeten E-Mails.</summary>
        [MaxLength(320)]
        public string FromAddress { get; set; } = string.Empty;

        /// <summary>Anzeigename des Absenders (optional).</summary>
        [MaxLength(255)]
        public string? FromName { get; set; }

        // --- Automatischer Versand ---

        /// <summary>
        /// Hauptschalter für den E-Mail-Versand. Ist er deaktiviert, wird beim Statuswechsel
        /// keine E-Mail versendet (der Testversand bleibt möglich).
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
