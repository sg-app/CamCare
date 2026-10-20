using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    /// <summary>
    /// Protokoll der automatisch versendeten Status-E-Mails. Der Eintrag dient zugleich
    /// als Sperre gegen Mehrfachversand: Solange ein erfolgreicher Versand für einen
    /// Auftrag und einen Status protokolliert ist, wird für diesen Status keine
    /// weitere E-Mail versendet – auch dann nicht, wenn der Status mehrfach geändert wird.
    /// </summary>
    public class RepairOrderMailLog
    {
        public int Id { get; set; }

        public int RepairOrderId { get; set; }

        /// <summary>
        /// Verwendete Vorlage. Wird nach dem Löschen der Vorlage auf <c>null</c> gesetzt,
        /// damit das Protokoll erhalten bleibt.
        /// </summary>
        public int? MailTemplateId { get; set; }

        /// <summary>Status, der den Versand ausgelöst hat (Snapshot, kein Fremdschlüssel).</summary>
        public int RepairOrderStatusId { get; set; }

        /// <summary>Name des Status zum Zeitpunkt des Versands (Snapshot).</summary>
        [MaxLength(100)]
        public string? StatusName { get; set; }

        /// <summary>Empfängeradressen (kommagetrennt) zum Zeitpunkt des Versands.</summary>
        [MaxLength(1000)]
        public string Recipients { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Subject { get; set; } = string.Empty;

        public bool IsSuccess { get; set; }

        /// <summary>Fehlermeldung, falls der Versand fehlgeschlagen ist.</summary>
        [MaxLength(2000)]
        public string? ErrorMessage { get; set; }

        public DateTime SentAt { get; set; }

        public virtual RepairOrder RepairOrder { get; set; } = null!;

        public virtual MailTemplate? MailTemplate { get; set; }
    }
}
