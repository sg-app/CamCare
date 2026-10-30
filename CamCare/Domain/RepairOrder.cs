using System.ComponentModel.DataAnnotations;

namespace CamCare.Domain
{
    public class RepairOrder : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public bool? IsCamera { get; set; }
        public string? SerialNumber { get; set; }
        public bool SerialNumberNeedsMaintenance { get; set; }
        public bool? PriorityRepair { get; set; }
        public bool WithoutCostestimate { get; set; }
        public string? PiceOfEquipment { get; set; }
        public int RepairOrderStatusId { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public string? ShippingMethodDescription { get; set; }
        public string? CustomerDescription { get; set; }
        public bool Warranty { get; set; }
        public PackagingType PackagingType { get; set; }
        public int? PackagingLength { get; set; }
        public int? PackagingWidth { get; set; }
        public int? PackagingHeight { get; set; }
        public string? PackagingComment { get; set; }
        public int? LogisticProviderId { get; set; }

        /// <summary>
        /// Ausgewählte Lieferanschrift des Auftrags. Die Anschrift selbst wird am Kunden
        /// (<see cref="DeliveryAddress.CustomerId"/>) gespeichert und kann wiederverwendet werden.
        /// </summary>
        public int? DeliveryAddressId { get; set; }

        /// <summary>
        /// E-Mail-Adressen des Auftrags. Die Adressen selbst werden am Kunden
        /// (<see cref="EmailAddress.CustomerId"/>) gespeichert und können daher für
        /// weitere Aufträge wiederverwendet werden.
        /// </summary>
        public virtual ICollection<EmailAddress> EmailAddresses { get; set; } = [];

        /// <summary>
        /// Alle dem Auftrag zugeordneten Seriennummern. Die Hauptseriennummer
        /// (<see cref="SerialNumber"/>) wird aus der als <see cref="RepairOrderSerial.IsPrimary"/>
        /// markierten Zeile abgeleitet.
        /// </summary>
        public virtual ICollection<RepairOrderSerial> RepairOrderSerials { get; set; } = [];

        public string? OrderNumber { get; set; }
        public string? QuoteNumber { get; set; }
        public string? DeliveryNoteNumber { get; set; }
        public DateTime? ArrivedAt { get; set; }

        /// <summary>
        /// Zeitstempel der letzten Statusänderung. Wird bei jeder Änderung des
        /// <see cref="RepairOrderStatusId"/> auf den aktuellen Zeitpunkt gesetzt.
        /// </summary>
        public DateTime? CurrentStateAt { get; set; }

        public virtual RepairOrderStatus RepairOrderStatus { get; set; } = null!;
        public virtual LogisticProvider LogisticProvider { get; set; } = null!;
        public virtual DeliveryAddress? DeliveryAddress { get; set; }
        public virtual ICollection<Defective> Defectives { get; set; } = [];
        public virtual ICollection<Work> Works { get; set; } = [];
        public virtual ICollection<IncludedComponent> IncludedComponents { get; set; } = [];
        public virtual ICollection<RepairPosition> RepairPositions { get; set; } = [];
        public virtual ICollection<RepairOrderRepairPosition> RepairOrderRepairPositions { get; set; } = [];

        /// <summary>
        /// Zuordnungen zu Defekten inklusive der am Auftrag gespeicherten Reihenfolge
        /// (<see cref="RepairOrderDefective.SortOrder"/>).
        /// </summary>
        public virtual ICollection<RepairOrderDefective> RepairOrderDefectives { get; set; } = [];

        /// <summary>
        /// Zuordnungen zu ausgeführten Arbeiten inklusive der am Auftrag gespeicherten
        /// Reihenfolge (<see cref="RepairOrderWork.SortOrder"/>).
        /// </summary>
        public virtual ICollection<RepairOrderWork> RepairOrderWorks { get; set; } = [];
        public virtual ICollection<Employee> Employees { get; set; } = [];
        public virtual ICollection<RepairOrderStatusHistory> RepairOrderStatusHistory { get; set; } = [];
        public virtual ICollection<DataStore> DataStores { get; set; } = [];

    }
}
