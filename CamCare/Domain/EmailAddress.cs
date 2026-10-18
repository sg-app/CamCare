using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    /// <summary>
    /// E-Mail-Adresse eines Kunden. Die Adresse wird am Kunden (Amicron-Kundennummer,
    /// siehe <see cref="CustomerId"/>) gespeichert und kann für weitere Aufträge
    /// wiederverwendet werden (z. B. für den automatisierten Mailversand).
    /// </summary>
    public class EmailAddress : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Kundennummer (LfdNr) aus dem Amicron-System, zu dem die E-Mail-Adresse gehört.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// E-Mail-Adresse (z. B. "name@firma.de").
        /// </summary>
        public string Address { get; set; } = string.Empty;

        public virtual ICollection<RepairOrder> RepairOrders { get; set; } = [];
    }
}
