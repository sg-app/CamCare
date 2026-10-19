using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    /// <summary>
    /// Seriennummer, die einem Reparaturauftrag zugeordnet ist. Ein Auftrag kann mehrere
    /// Seriennummern enthalten. Die erste Seriennummer (<see cref="IsPrimary"/>) wird zusätzlich
    /// als Hauptseriennummer in <see cref="RepairOrder.SerialNumber"/> gespiegelt, damit
    /// bestehende Abfragen (offene Aufträge, Suche nach Seriennummer) unverändert funktionieren.
    /// </summary>
    public class RepairOrderSerial : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int RepairOrderId { get; set; }

        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// True, wenn die Seriennummer manuell erfasst wurde und von einem Mitarbeiter
        /// in den Stammdaten nachgepflegt werden muss.
        /// </summary>
        public bool NeedsMaintenance { get; set; }

        /// <summary>
        /// True, wenn die Seriennummer einem anderen Kunden als dem Auftragskunden zugeordnet ist.
        /// Die Auswahl ist erlaubt, wird dem Benutzer aber kenntlich gemacht.
        /// </summary>
        public bool IsForeignCustomer { get; set; }

        public string? ArticleNumber { get; set; }

        public string? ArticleDescription { get; set; }

        /// <summary>Kundennummer des Kunden, dem die Seriennummer in Amicron zugeordnet ist.</summary>
        public string? OwnerCustomerNumber { get; set; }

        /// <summary>Name des Kunden, dem die Seriennummer in Amicron zugeordnet ist.</summary>
        public string? OwnerCustomerName { get; set; }

        /// <summary>
        /// True für die Hauptseriennummer des Auftrags. Diese wird nach
        /// <see cref="RepairOrder.SerialNumber"/> gespiegelt.
        /// </summary>
        public bool IsPrimary { get; set; }

        public virtual RepairOrder RepairOrder { get; set; } = null!;
    }
}
