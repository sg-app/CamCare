
namespace CamCare.Domain
{
    public class RepairOrderStatus : IAuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public string? BackgroundColor { get; set; }
        public string? FontColor { get; set; }



        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
