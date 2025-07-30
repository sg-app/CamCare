using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class Camera : IAuditableEntity
    {
        [Key]
        [MaxLength(100)]
        public string SerialNumber { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public int CameraTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<RepairOrder>? RepairOrders { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual CameraType CameraType { get; set; } = null!;
    }
}
