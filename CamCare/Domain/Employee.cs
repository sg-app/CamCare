using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class Employee : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        public virtual ICollection<RepairOrder> RepairOrders { get; set; } = [];
    }
}
