using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class Defective : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<RepairOrder> RepairOrders { get; set; } = [];

    }
}
