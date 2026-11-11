using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class RepairOrderStatusHistory
    {
        public int Id { get; set; }
        public int RepairOrderId { get; set; }
        public int? RepairOrderStatusId { get; set; }
        [MaxLength(100)]
        public string? StatusName { get; set; }
        [MaxLength(30)]
        public string? BackgroundColor { get; set; }
        [MaxLength(30)]
        public string? FontColor { get; set; }
        public DateTime ChangedAt { get; set; }

        /// <summary>
        /// Name der Benutzergruppe (admin/buero/werkstatt), die den Status geändert hat.
        /// Snapshot-Feld analog zu <see cref="StatusName"/>: bleibt auch erhalten, wenn eine
        /// Benutzergruppe umbenannt oder entfernt wird.
        /// </summary>
        [MaxLength(50)]
        public string? ChangedByUserGroup { get; set; }

        public virtual RepairOrderStatus? RepairOrderStatus { get; set; }

    }
}
