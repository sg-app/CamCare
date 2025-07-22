using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class AddressType : IAuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}