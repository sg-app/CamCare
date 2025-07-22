using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class RepairOrder : IAuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public ICollection<RepairPosition>? RepairPositions { get; set; }
        public ICollection<RepairOrderRepairPosition> RepairOrderRepairPositions { get; set; } = [];

        public Customer Customer { get; set; } = null!;



        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
