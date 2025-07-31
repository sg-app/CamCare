using System;
using System.Collections.Generic;

namespace CamCare.Models
{
    public class RepairPositionVm
    {
        public int Id { get; set; }
        public string? Artikelnummer { get; set; }
        public string Description { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<RepairOrderVm>? RepairOrders { get; set; }

        public override string ToString() => $"[{Id}] {Quantity} - {Description}";
    }
}