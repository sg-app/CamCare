namespace CamCare.Components.RepairOrderComponents
{
    /// <summary>
    /// Anfrage zum Verschieben eines Eintrags innerhalb einer Auftrags-Zuordnung
    /// (Ergebnis eines Drag&amp;Drop-Vorgangs).
    /// </summary>
    /// <param name="From">Bisherige Position des Eintrags (0-basiert).</param>
    /// <param name="To">Neue Position des Eintrags (0-basiert).</param>
    public readonly record struct AssignmentMoveRequest(int From, int To);
}
