namespace CamCare.Models
{
    public class RepairOrderStatusUsageVm
    {
        public int HistoryCount { get; set; }
        public int RepairOrderCount { get; set; }

        public bool IsUsed => HistoryCount > 0 || RepairOrderCount > 0;
    }
}
