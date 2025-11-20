using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class RepairPosition : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string? Artikelnummer { get; set; }
        public string Description { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool? FromAmicron { get; set; }

        public virtual ICollection<RepairOrder> RepairOrders { get; set; } = [];
        public virtual ICollection<RepairOrderRepairPosition> RepairOrderRepairPositions { get; set; } = [];

    }
}
