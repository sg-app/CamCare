namespace CamCare.Domain
{
    /// <summary>
    /// Zuordnung einer ausgeführten Arbeit (<see cref="Work"/>) zu einem Reparaturauftrag.
    /// Die Zuordnung ist eine eigene Entität, damit die Reihenfolge der am Auftrag erfassten
    /// Arbeiten gespeichert werden kann (<see cref="SortOrder"/>).
    /// </summary>
    public class RepairOrderWork : IRepairOrderAssignment
    {
        public int RepairOrderId { get; set; }
        public int WorkId { get; set; }

        /// <summary>Reihenfolge des Eintrags innerhalb des Auftrags (beginnend bei 0).</summary>
        public int SortOrder { get; set; }

        public virtual RepairOrder RepairOrder { get; set; } = null!;
        public virtual Work Work { get; set; } = null!;
    }
}
