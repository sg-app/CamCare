using System.ComponentModel.DataAnnotations;

namespace CamCare.Models
{
    public class CameraTypeVm
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<CameraVm>? Cameras { get; set; }
    }
}
