using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CamCare.Models
{
    public class RepairOrderStatusVm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name ist erforderlich.")]
        [MaxLength(100, ErrorMessage = "Maximal {1} Zeichen erlaubt.")]
        [MinLength(3, ErrorMessage = "Mindestens {1} Zeichen erforderlich.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Beschreibung ist erforderlich.")]
        [MaxLength(300, ErrorMessage = "Maximal {1} Zeichen erlaubt.")]
        [MinLength(3, ErrorMessage = "Mindestens {1} Zeichen erforderlich.")]
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        [MaxLength(30)]
        public string? BackgroundColor { get; set; }
        [MaxLength(30)]
        public string? FontColor { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;
        public bool IsOrderClose { get; set; }
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

        public override string ToString() => $"[{Id}] {Name}";
    }
}