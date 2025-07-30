using System.ComponentModel.DataAnnotations;

namespace CamCare.Models
{
    public class AddressVm
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public AddressType AddressType { get; set; }
        [MaxLength(250)]
        public string Street { get; set; } = string.Empty;
        [MaxLength(10)]
        public string HouseNumber { get; set; } = string.Empty;
        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? State { get; set; }
        [MaxLength(100)]
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public CustomerVm? Customer { get; set; }

        public override string ToString() => $"[{Id}] CustomerId: {CustomerId}, AddressType: {AddressType}";
    }
}