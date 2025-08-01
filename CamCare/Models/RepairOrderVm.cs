using System;
using System.Collections.Generic;

namespace CamCare.Models
{
    public class RepairOrderVm
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string CameraSerialNumber { get; set; } = string.Empty;
        public int RepairOrderStatusId { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public int? LogisticProviderId { get; set; }
        public string? ShippingMethodDescription { get; set; }
        public DateTime ArrivedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public CustomerVm? Customer { get; set; }
        public CameraVm? Camera { get; set; }
        public RepairOrderStatusVm? RepairOrderStatus { get; set; }
        public LogisticProviderVm? LogisticProvider { get; set; }
        public ICollection<DefectiveVm>? Defectives { get; set; }
        public ICollection<RepairPositionVm>? RepairPositions { get; set; }

        public override string ToString() => $"[{Id}] CameraId: {CameraSerialNumber}";
    }
}