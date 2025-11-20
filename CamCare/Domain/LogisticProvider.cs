using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class LogisticProvider : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<RepairOrder> RepairOrders { get; set; } = [];
    }
}
