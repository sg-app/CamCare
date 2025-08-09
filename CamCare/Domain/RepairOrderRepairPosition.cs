namespace CamCare.Domain
{
    public class RepairOrderRepairPosition
    {
        public int RepairOrderId { get; set; }
        public int RepairPostionId { get; set; }
        public decimal Quantity { get; set; }

        public virtual RepairOrder RepairOrder { get; set; } = null!;
        public virtual RepairPosition RepairPosition { get; set; } = null!;
    }
}
