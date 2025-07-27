using System;
using System.Collections.Generic;

namespace CamCare.Models
{
    public class RepairPositionVm
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<RepairOrderVm>? RepairOrders { get; set; }
        public ICollection<RepairOrderRepairPositionVm>? RepairOrderRepairPositions { get; set; }

        public override string ToString() => $"[{Id}] {Description}";
    }
}