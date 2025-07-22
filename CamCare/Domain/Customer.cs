using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class Customer : IAuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        [MaxLength(250)]
        public string? Email { get; set; }
        [MaxLength(100)]
        public string? Phone { get; set; }

        public ICollection<Address> Addresses = [];



        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
