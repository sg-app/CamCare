namespace CamCare.Models
{
    /// <summary>
    /// Protokoll eines automatisch versendeten Status-E-Mails (Anzeige in der
    /// Konfigurationsoberfläche).
    /// </summary>
    public class RepairOrderMailLogVm
    {
        public int Id { get; set; }

        public int RepairOrderId { get; set; }

        /// <summary>Verwendete Vorlage (leer, wenn die Vorlage zwischenzeitlich gelöscht wurde).</summary>
        public int? MailTemplateId { get; set; }

        public int RepairOrderStatusId { get; set; }

        public string? StatusName { get; set; }

        public string Recipients { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }

        /// <summary>Zeitpunkt des Versands (lokale Zeit).</summary>
        public DateTime SentAt { get; set; }

        public string ResultText => IsSuccess ? "Versendet" : $"Fehlgeschlagen: {ErrorMessage}";
    }
}
