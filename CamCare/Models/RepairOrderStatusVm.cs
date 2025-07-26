using System.Text;

namespace CamCare.Models
{
    public class RepairOrderStatusVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public string? BackgroundColor { get; set; }
        public string? FontColor { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string BadageStyle => GetBadageStyle();

        private string GetBadageStyle()
        {
            var sb = new StringBuilder();
            if (BackgroundColor != null)
            {
                sb.Append($"background-color: {BackgroundColor}; ");
            }
            if (FontColor != null)
            {
                sb.Append($"color: {FontColor}; ");
            }
            return sb.ToString();
        }
    }
}