using System.Text;

namespace CamCare.Models
{
    public class RepairOrderStatusHistoryVm
    {
        public int Id { get; set; }
        public int RepairOrderId { get; set; }
        public int? RepairOrderStatusId { get; set; }
        public string? StatusName { get; set; }
        public string? BackgroundColor { get; set; }
        public string? FontColor { get; set; }
        public DateTime ChangedAt { get; set; }

        public RepairOrderStatusVm? RepairOrderStatus { get; set; }

        /// <summary>
        /// Anzeigename: bevorzugt den zum Zeitpunkt der Änderung gespeicherten Namen (Snapshot),
        /// andernfalls – solange der Status noch existiert – den aktuellen Statusnamen.
        /// </summary>
        public string? DisplayStatusName => StatusName ?? RepairOrderStatus?.Name;

        /// <summary>
        /// Badge-Stil: bevorzugt die zum Zeitpunkt der Änderung gespeicherten Farben (Snapshot),
        /// andernfalls – solange der Status noch existiert – die aktuellen Statusfarben.
        /// </summary>
        public string? BadageStyle
        {
            get
            {
                var sb = new StringBuilder();
                var bg = BackgroundColor ?? RepairOrderStatus?.BackgroundColor;
                var fg = FontColor ?? RepairOrderStatus?.FontColor;
                if (!string.IsNullOrEmpty(bg))
                    sb.Append($"background-color: {bg}; ");
                if (!string.IsNullOrEmpty(fg))
                    sb.Append($"color: {fg}; ");
                return sb.ToString();
            }
        }
    }
}
