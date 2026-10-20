namespace CamCare.Models
{
    /// <summary>
    /// Seriennummer eines Reparaturauftrags (siehe <see cref="Domain.RepairOrderSerial"/>).
    /// </summary>
    public class RepairOrderSerialVm
    {
        public int Id { get; set; }
        public int RepairOrderId { get; set; }
        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>True, wenn die Seriennummer manuell erfasst wurde und nachgepflegt werden muss.</summary>
        public bool NeedsMaintenance { get; set; }

        /// <summary>True, wenn die Seriennummer einem anderen Kunden als dem Auftragskunden zugeordnet ist.</summary>
        public bool IsForeignCustomer { get; set; }

        public string? ArticleNumber { get; set; }
        public string? ArticleDescription { get; set; }

        public string? OwnerCustomerNumber { get; set; }
        public string? OwnerCustomerName { get; set; }

        /// <summary>True für die Hauptseriennummer des Auftrags.</summary>
        public bool IsPrimary { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public override string ToString() => SerialNumber;
    }
}
