
using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class RepairOrderStatus : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(300)]
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        [MaxLength(30)]
        public string? BackgroundColor { get; set; }
        [MaxLength(30)]
        public string? FontColor { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;
        public bool IsOrderClose { get; set; } = false;


        public override string ToString() => $"[{Id}] {Name}";
    }
}
