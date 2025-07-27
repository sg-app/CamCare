namespace CamCare.Domain
{
    public class RepairOrderRepairPosition
    {
        public int RepairOrderId { get; set; }
        public int RepairPositionId { get; set; }
        public virtual RepairOrder RepairOrder { get; set; } = null!;
        public virtual RepairPosition RepairPosition { get; set; } = null!;

        public int DisplayOrder { get; set; }
    }
}
