using System;

namespace CamCare.Models
{
    public class RepairOrderRepairPositionVm
    {
        public int RepairOrderId { get; set; }
        public int RepairPositionId { get; set; }
        public int DisplayOrder { get; set; }

        public RepairOrderVm? RepairOrder { get; set; }
        public RepairPositionVm? RepairPosition { get; set; }

        public override string ToString() => $"[{RepairOrderId}:{RepairPositionId}] {DisplayOrder}";
    }
}