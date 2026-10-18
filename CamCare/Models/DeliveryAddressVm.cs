using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CamCare.Models
{
    /// <summary>
    /// Lieferanschrift eines Kunden. Wird für die Auswahl beim Reparaturauftrag
    /// und für die Erfassung einer neuen Anschrift verwendet.
    /// </summary>
    public class DeliveryAddressVm
    {
        [Key]
        public int Id { get; set; }

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

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Kennzeichnet eine Anschrift, die noch nicht in der Datenbank gespeichert ist.
        /// </summary>
        public bool IsNew => Id <= 0;

        /// <summary>
        /// Anzeigename für Auswahllisten, z. B. "Firma Muster (Herr Meier), Musterweg 1, 12345 Musterstadt".
        /// </summary>
        public string DisplayName
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(Recipient);

                if (!string.IsNullOrWhiteSpace(ContactPerson))
                    sb.Append(" (").Append(ContactPerson.Trim()).Append(')');

                sb.Append(", ").Append(Street).Append(", ")
                  .Append(PostalCode).Append(' ').Append(City);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Mehrzeilige Darstellung der Anschrift (Empfänger, Ansprechpartner, Straße, PLZ/Ort, Land).
        /// </summary>
        public string FullAddress
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(Recipient);

                if (!string.IsNullOrWhiteSpace(ContactPerson))
                    sb.AppendLine($"Ansprechpartner: {ContactPerson.Trim()}");

                sb.AppendLine(Street);
                sb.Append($"{PostalCode} {City}".Trim());

                if (!string.IsNullOrWhiteSpace(Country))
                    sb.Append(Environment.NewLine).Append(Country.Trim());

                return sb.ToString();
            }
        }

        public override string ToString() => $"[{Id}] {DisplayName}";
    }
}
