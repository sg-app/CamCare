namespace CamCare.Domain
{
    /// <summary>
    /// Zuordnung eines Stammdatensatzes (z. B. Defekt, Arbeit, Komponente, Position) zu einem
    /// Reparaturauftrag. Die Zuordnung ist eine eigene Entität, damit zusätzlich zur Zuordnung
    /// die Reihenfolge der am Auftrag erfassten Einträge gespeichert werden kann.
    /// </summary>
    public interface IRepairOrderAssignment
    {
        /// <summary>Reihenfolge des Eintrags innerhalb des Auftrags (beginnend bei 0).</summary>
        int SortOrder { get; set; }
    }
}
