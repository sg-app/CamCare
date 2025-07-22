using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class Parameter : IAuditableEntity
    {
        [Key]
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;


        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
