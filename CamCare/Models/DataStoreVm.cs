using System.ComponentModel.DataAnnotations;

namespace CamCare.Models
{
    public class DataStoreVm
    {
        [Key]
        public int Id { get; set; }
        public int RepairOrderId { get; set; }
        public string Filename { get; set; } = string.Empty;
        public string? Description { get; set; }
        public byte[]? Data { get; set; }
        public string? ObjectKey { get; set; }
        public long SizeBytes { get; set; }
        public string? DownloadUrl { get; set; }
        public string Type { get; set; } = string.Empty;

        public virtual RepairOrderVm RepairOrder { get; set; } = null!;

    }
}
