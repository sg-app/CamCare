
using System.ComponentModel.DataAnnotations;

namespace CamCare.Models
{
    public class CameraVm
    {
        [Required(ErrorMessage = "Seriennummer muss eingetragen sein.")]
        [MaxLength(100, ErrorMessage = "Maximal {1} Zeichen erlaubt.")]
        public string SerialNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Kamera-Typ muss ausgewählt sein.")]
        public int CameraTypeId { get; set; }
        public string? ArtikelName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<RepairOrderVm>? RepairOrders { get; set; }
        public CameraTypeVm? CameraType { get; set; }
        public Amicron.Adressen? Customer { get; set; }

        public override string ToString() => $"{SerialNumber}, CustomerId: {CustomerId}";

        public string DisplayName
            => CameraType is null
            ? $"{SerialNumber} {ArtikelName}"
            : $"[{CameraType.Name}] {SerialNumber}";
    }
}
