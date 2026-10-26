namespace CamCare.Models
{
    /// <summary>
    /// E-Mail-Vorlage für einen Status des Reparaturauftrags. Beim Erreichen dieses Status
    /// wird die Vorlage automatisch an die E-Mail-Adressen des Auftrags versendet.
    /// </summary>
    public class MailTemplateVm
    {
        public int Id { get; set; }

        public int RepairOrderStatusId { get; set; }

        public bool IsEnabled { get; set; } = true;

        public string SubjectTemplate { get; set; } = string.Empty;

        public string BodyTemplate { get; set; } = string.Empty;

        public int Order { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        /// <summary>Name des Status (nur zur Anzeige, wird beim Laden ergänzt).</summary>
        public string? StatusName { get; set; }

        /// <summary>Kennzeichnet eine noch nicht gespeicherte Vorlage.</summary>
        public bool IsNew => Id <= 0;

        public string DisplayName => $"[{Id}] {StatusName ?? "ohne Status"}";

        public override string ToString() => DisplayName;
    }
}
