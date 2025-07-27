using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class Defective
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
