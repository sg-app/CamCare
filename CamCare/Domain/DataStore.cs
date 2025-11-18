using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class DataStore : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int RepairOrderId { get; set; }
        public byte[] Data { get; set; } = default!;
        public string Type { get; set; } = string.Empty;

        public virtual RepairOrder RepairOrder { get; set; } = null!;

    }
}
