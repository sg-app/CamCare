using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class RepairOrder : IAuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int CameraId { get; set; }
        public virtual ICollection<RepairPosition>? RepairPositions { get; set; }
        public virtual ICollection<RepairOrderRepairPosition> RepairOrderRepairPositions { get; set; } = [];
        public DateTime ArrivedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Camera Camera { get; set; } = null!;
    }
}
