using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class LogisticProvider : IAuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<RepairOrder> RepairOrders { get; set; } = [];

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
