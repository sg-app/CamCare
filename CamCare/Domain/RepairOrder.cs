using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class RepairOrder : IAuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string CameraSerialNumber { get; set; } = string.Empty;
        public int RepairOrderStatusId { get; set; }
        public int? LogisticProviderId { get; set; }
        public DateTime ArrivedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Camera Camera { get; set; } = null!;
        public virtual RepairOrderStatus RepairOrderStatus { get; set; } = null!;
        public virtual LogisticProvider? LogisticProvider { get; set; }
        public virtual ICollection<Defective>? Defectives { get; set; }
        public virtual ICollection<RepairPosition>? RepairPositions { get; set; }
        public virtual ICollection<RepairOrderRepairPosition>? RepairOrderRepairPositions { get; set; }
    }
}
