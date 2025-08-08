using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class CameraType : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Camera>? Cameras { get; set; }
    }
}
