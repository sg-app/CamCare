using CamCare.Models.Amicron;

namespace CamCare.Models
{
    public class RepairOrderVm
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? CameraSerialNumber { get; set; }
        public string? PiceOfEquipment { get; set; }
        public int RepairOrderStatusId { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public int? LogisticProviderId { get; set; }
        public string? ShippingMethodDescription { get; set; }
        public string? OrderNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public string? DeliveryNoteNumber { get; set; }
        public DateTime ArrivedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Adressen? Customer { get; set; }
        public Serials? Serials { get; set; }
        public RepairOrderStatusVm? RepairOrderStatus { get; set; }
        public LogisticProviderVm? LogisticProvider { get; set; }
        public ICollection<DefectiveVm>? Defectives { get; set; }
        public ICollection<IncludedComponentVm>? IncludedComponents { get; set; }
        public ICollection<RepairPositionVm>? RepairPositions { get; set; }
        public ICollection<EmployeeVm>? Employees { get; set; }
        public ICollection<RepairOrderStatusHistoryVm>? RepairOrderStatusHistories { get; set; }
        public ICollection<DataStoreVm>? DataStores { get; set; }

        public override string ToString() => $"[{Id}] CameraId: {CameraSerialNumber}";
    }
}