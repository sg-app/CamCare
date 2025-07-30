using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class Customer : IAuditableEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? CompanyName { get; set; }
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }
        [MaxLength(250)]
        public string? Email { get; set; }
        [MaxLength(100)]
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public virtual ICollection<Address>? Addresses { get; set; }
        public virtual ICollection<Camera>? Cameras { get; set; }
    }
}
