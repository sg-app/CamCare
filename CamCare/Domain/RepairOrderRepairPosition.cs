using System.ComponentModel.DataAnnotations.Schema;

namespace CamCare.Domain
{
    public class RepairOrderRepairPosition
    {
        public int RepairOrderId { get; set; }
        public int RepairPostionId { get; set; }

        [Column(TypeName = "decimal(16,4)")]
        public decimal Quantity { get; set; }

        public virtual RepairOrder RepairOrder { get; set; } = null!;
        public virtual RepairPosition RepairPosition { get; set; } = null!;
    }
}
