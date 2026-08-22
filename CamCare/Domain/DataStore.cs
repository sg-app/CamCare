using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class DataStore : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int RepairOrderId { get; set; }
        public string Filename { get; set; } = string.Empty;
        public string? Description { get; set; }
        public byte[]? Data { get; set; }
        public string? ObjectKey { get; set; }
        public long SizeBytes { get; set; }
        public string Type { get; set; } = string.Empty;

        public virtual RepairOrder RepairOrder { get; set; } = null!;

    }
}
