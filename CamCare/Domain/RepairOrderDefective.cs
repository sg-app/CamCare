namespace CamCare.Domain
{
    /// <summary>
    /// Zuordnung eines Defekts (<see cref="Defective"/>) zu einem Reparaturauftrag. Die
    /// Zuordnung ist eine eigene Entität, damit die Reihenfolge der am Auftrag erfassten
    /// Defekte gespeichert werden kann (<see cref="SortOrder"/>).
    /// </summary>
    public class RepairOrderDefective : IRepairOrderAssignment
    {
        public int RepairOrderId { get; set; }
        public int DefectiveId { get; set; }

        /// <summary>Reihenfolge des Eintrags innerhalb des Auftrags (beginnend bei 0).</summary>
        public int SortOrder { get; set; }

        public virtual RepairOrder RepairOrder { get; set; } = null!;
        public virtual Defective Defective { get; set; } = null!;
    }
}
