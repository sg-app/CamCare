using CamCare.Domain;

namespace CamCare.Models
{
    public class AddressTypeVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Default { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public override string ToString() => $"[{Id}] {Name}";
    }
}