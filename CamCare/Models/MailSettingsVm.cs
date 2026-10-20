namespace CamCare.Models
{
    /// <summary>
    /// Konfiguration des E-Mail-Versands: SMTP-Zugang sowie die Vorlage, die beim
    /// Erreichen eines konfigurierbaren Status automatisch an die E-Mail-Adressen
    /// des Reparaturauftrags versendet wird.
    /// </summary>
    public class MailSettingsVm
    {
        public int Id { get; set; } = 1;

        // --- SMTP-Server ---

        public string SmtpHost { get; set; } = string.Empty;

        public int SmtpPort { get; set; } = 587;

        public MailSecurityMode Security { get; set; } = MailSecurityMode.StartTls;

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public int TimeoutSeconds { get; set; } = 30;

        public string FromAddress { get; set; } = string.Empty;

        public string? FromName { get; set; }

        // --- Automatischer Versand ---

        /// <summary>
        /// Hauptschalter für den E-Mail-Versand. Ist er deaktiviert, wird beim Statuswechsel
        /// keine E-Mail versendet (der Testversand bleibt möglich).
        /// </summary>
        public bool IsEnabled { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public override string ToString() => $"[{Id}] SMTP: {SmtpHost}:{SmtpPort}";
    }
}
