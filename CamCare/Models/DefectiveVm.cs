using System.ComponentModel.DataAnnotations;

namespace CamCare.Models
{
    public class DefectiveVm
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<RepairOrderVm> RepairOrders { get; set; } = [];

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public override string ToString()
            => $"[{Id}] {Description}";
    }
}
