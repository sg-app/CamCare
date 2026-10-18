using System.ComponentModel.DataAnnotations;

namespace CamCare.Models
{
    /// <summary>
    /// E-Mail-Adresse eines Kunden. Wird für die Auswahl beim Reparaturauftrag
    /// und für die Erfassung einer neuen Adresse verwendet.
    /// </summary>
    public class EmailAddressVm
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        /// <summary>
        /// E-Mail-Adresse (z. B. "name@firma.de").
        /// </summary>
        public string Address { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Kennzeichnet eine Adresse, die noch nicht in der Datenbank gespeichert ist.
        /// </summary>
        public bool IsNew => Id <= 0;

        /// <summary>
        /// Anzeigename für Auswahllisten (die E-Mail-Adresse selbst).
        /// </summary>
        public string DisplayName => Address;

        public override string ToString() => $"[{Id}] {Address}";
    }
}
