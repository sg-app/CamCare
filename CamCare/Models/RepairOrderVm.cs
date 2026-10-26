using CamCare.Models.Amicron;

namespace CamCare.Models
{
    public class RepairOrderVm
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public bool? IsCamera { get; set; }
        public string? CameraSerialNumber { get; set; }
        public bool SerialNumberNeedsMaintenance { get; set; }
        public bool? PriorityRepair { get; set; } = false;
        public bool WithoutCostestimate { get; set; } = false;
        public string? PiceOfEquipment { get; set; }
        public int RepairOrderStatusId { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public PackagingType PackagingType { get; set; }
        public int? PackagingLength { get; set; }
        public int? PackagingWidth { get; set; }
        public int? PackagingHeight { get; set; }
        public string? PackagingComment { get; set; }
        public int? LogisticProviderId { get; set; }

        /// <summary>
        /// Ausgewählte Lieferanschrift des Auftrags (Id der gespeicherten Anschrift).
        /// </summary>
        public int? DeliveryAddressId { get; set; }

        public string? ShippingMethodDescription { get; set; }
        public string? CustomerDescription { get; set; }
        public bool Warranty { get; set; }
        public string? OrderNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public string? DeliveryNoteNumber { get; set; }
        public DateTime? ArrivedAt { get; set; }
        /// <summary>
        /// Zeitstempel der letzten Statusänderung (lokale Zeit).
        /// </summary>
        public DateTime? CurrentStateAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Alle dem Auftrag zugeordneten Seriennummern. Die Hauptseriennummer
        /// (<see cref="CameraSerialNumber"/>) wird beim Speichern aus der ersten Zeile abgeleitet.
        /// </summary>
        public ICollection<RepairOrderSerialVm>? RepairOrderSerials { get; set; }

        public Adressen? Customer { get; set; }
        public Serials? Serials { get; set; }
        public RepairOrderStatusVm? RepairOrderStatus { get; set; }
        public LogisticProviderVm? LogisticProvider { get; set; }

        /// <summary>
        /// Ausgewählte bzw. neu erfasste Lieferanschrift. Eine neu erfasste Anschrift
        /// (Id = 0) wird beim Speichern des Auftrags für den Kunden hinterlegt.
        /// </summary>
        public DeliveryAddressVm? DeliveryAddress { get; set; }

        /// <summary>
        /// Ausgewählte bzw. neu erfasste E-Mail-Adressen des Auftrags. Neu erfasste
        /// Adressen (Id = 0) werden beim Speichern des Auftrags für den Kunden hinterlegt.
        /// </summary>
        public ICollection<EmailAddressVm>? EmailAddresses { get; set; }

        public ICollection<DefectiveVm>? Defectives { get; set; }
        public ICollection<WorkVm>? Works { get; set; }
        public ICollection<IncludedComponentVm>? IncludedComponents { get; set; }
        public ICollection<RepairPositionVm>? RepairPositions { get; set; }
        public ICollection<EmployeeVm>? Employees { get; set; }
        public ICollection<RepairOrderStatusHistoryVm>? RepairOrderStatusHistories { get; set; }
        public ICollection<DataStoreVm>? DataStores { get; set; }

        /// <summary>Anzeigename für Auswahllisten.</summary>
        public string DisplayName => $"[{Id}] {OrderNumber ?? "ohne Auftragsnummer"} – {CameraSerialNumber ?? "ohne Seriennummer"}";

        public override string ToString() => $"[{Id}] CameraId: {CameraSerialNumber}";
    }
}
