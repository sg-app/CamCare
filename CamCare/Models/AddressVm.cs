using System.ComponentModel.DataAnnotations;

namespace CamCare.Models
{
    public class AddressVm
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public AddressType AddressType { get; set; }
        public string Street { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public CustomerVm? Customer { get; set; }

        public override string ToString() => $"[{Id}] CustomerId: {CustomerId}, AddressType: {AddressType}";
    }
}