using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    /// <summary>
    /// Eine E-Mail-Vorlage, die beim Erreichen eines Status automatisch an die im
    /// Reparaturauftrag hinterlegten E-Mail-Adressen versendet wird.
    /// Es können beliebig viele Vorlagen (auch mehrere je Status) konfiguriert werden,
    /// z. B. eine Vorlage für den Status "Eingetroffen" und eine für "Versendet".
    /// </summary>
    public class MailTemplate : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Status des Reparaturauftrags, bei dessen Erreichen die E-Mail versendet wird.</summary>
        public int RepairOrderStatusId { get; set; }

        /// <summary>Kennzeichnet, ob die Vorlage beim Statuswechsel verwendet wird.</summary>
        public bool IsEnabled { get; set; }

        /// <summary>Betreff-Vorlage der E-Mail (Platzhalter in <c>{{Token}}</c>-Schreibweise).</summary>
        [MaxLength(300)]
        public string SubjectTemplate { get; set; } = string.Empty;

        /// <summary>HTML-Inhalt-Vorlage der E-Mail (Platzhalter in <c>{{Token}}</c>-Schreibweise).</summary>
        public string BodyTemplate { get; set; } = string.Empty;

        /// <summary>Sortierreihenfolge in der Übersicht.</summary>
        public int Order { get; set; }

        public virtual RepairOrderStatus RepairOrderStatus { get; set; } = null!;
    }
}
