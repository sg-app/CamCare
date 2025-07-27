using System;
using System.Collections.Generic;

namespace CamCare.Models
{
    public class RepairOrderVm
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public DateTime ArrivedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public CameraVm? Camera { get; set; }
        public ICollection<RepairOrderRepairPositionVm>? RepairOrderRepairPositions { get; set; }

        public override string ToString() => $"[{Id}] CameraId: {CameraId}";
    }
}