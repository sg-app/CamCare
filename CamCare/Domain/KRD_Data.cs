using System.ComponentModel.DataAnnotations.Schema;

namespace CamCare.Domain
{
    [Table("Fahrzeugliste")]
    public class Krd_Data
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("FIN")]
        public string? CameraSerial { get; set; }
        [Column("TYP")]
        public string? Description { get; set; }
        [Column("SN")]
        public string? CameraType { get; set; }
        [Column("GWNr")]
        public string? AdditionalComponents { get; set; }
        [Column("AuftragNr")]
        public string? AmicronNumbers { get; set; }
        [Column("Anlieferung")]
        public DateTime? ArrivedAt { get; set; }
        [Column("Kunde")]
        public string? Kunde { get; set; }
        [Column("Techniker")]
        public string? Techniker { get; set; }
    }
}
