using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CamCare.Models
{
    public class CustomerVm
    {
        [MinLength(2)]
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

        public ICollection<AddressVm>? Addresses { get; set; }
        public ICollection<CameraVm>? Cameras { get; set; }

        public override string ToString() 
            => CompanyName is not null 
            ? $"[{Id}] {CompanyName}" 
            : $"[{Id}] {FirstName} {LastName}";
        
        public string DisplayName 
            => CompanyName is not null 
            ? $"[{Id}] {CompanyName}" 
            : $"[{Id}] {FirstName} {LastName}";
    }
}