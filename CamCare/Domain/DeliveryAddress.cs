using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    /// <summary>
    /// Lieferanschrift eines Kunden. Die Anschrift wird am Kunden (Amicron-Kundennummer,
    /// siehe <see cref="CustomerId"/>) gespeichert und kann für weitere Aufträge
    /// wiederverwendet werden.
    /// </summary>
    public class DeliveryAddress : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Kundennummer (LfdNr) aus dem Amicron-System, zu dem die Lieferanschrift gehört.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Empfänger der Lieferung (Name oder Firma).
        /// </summary>
        public string Recipient { get; set; } = string.Empty;

        /// <summary>
        /// Ansprechpartner beim Empfänger (optional).
        /// </summary>
        public string? ContactPerson { get; set; }

        public string Street { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string? Country { get; set; }

        public virtual ICollection<RepairOrder> RepairOrders { get; set; } = [];
    }
}
