using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class RepairOrder : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? SerialNumber { get; set; }
        public string? PiceOfEquipment { get; set; }
        public string? AdditionalComponents { get; set; }
        public int RepairOrderStatusId { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public int? LogisticProviderId { get; set; }
        public string? OrderNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public string? DeliveryNoteNumber { get; set; }
        public DateTime ArrivedAt { get; set; } = DateTime.UtcNow;

        public virtual RepairOrderStatus RepairOrderStatus { get; set; } = null!;
        public virtual LogisticProvider LogisticProvider { get; set; } = null!;
        public virtual ICollection<Defective> Defectives { get; set; } = [];
        public virtual ICollection<RepairPosition> RepairPositions { get; set; } = [];
        public virtual ICollection<RepairOrderRepairPosition> RepairOrderRepairPositions { get; set; } = [];
        public virtual ICollection<Employee> Employees { get; set; } = [];
        public virtual ICollection<RepairOrderStatusHistory> RepairOrderStatusHistory { get; set; } = [];

    }
}
