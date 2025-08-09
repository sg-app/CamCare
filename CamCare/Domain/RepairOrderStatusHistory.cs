namespace CamCare.Domain
{
    public class RepairOrderStatusHistory
    {
        public int Id { get; set; }
        public int RepairOrderId { get; set; }
        public int RepairOrderStatusId { get; set; }
        public DateTime ChangedAt { get; set; }

        public virtual RepairOrderStatus RepairOrderStatus { get; set; } = null!;

    }
}
