
namespace CamCare.Models
{
    public class RepairOrderStatusHistoryVm
    {
        public int Id { get; set; }
        public int RepairOrderId { get; set; }
        public int RepairOrderStatusId { get; set; }
        public DateTime ChangedAt { get; set; }

        public RepairOrderStatusVm? RepairOrderStatus { get; set; }

    }
}
