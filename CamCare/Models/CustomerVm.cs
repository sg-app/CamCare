using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CamCare.Models
{
    public class CustomerVm
    {
        public string Id { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
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