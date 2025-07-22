using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class RepairPosition :IAuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;


        public ICollection<RepairOrder> RepairOrders { get; set; } = [];
        public ICollection<RepairOrderRepairPosition> RepairOrderRepairPositions { get; set; } = [];

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
