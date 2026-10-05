namespace CamCare.Models
{
    public enum RepairOrderChangeType
    {
        Created,
        Updated
    }

    /// <summary>
    /// Wird über den RepairOrder-Change-Bus publiziert, sobald ein RepairOrder
    /// angelegt oder geändert wurde.
    /// </summary>
    public sealed record RepairOrderChangedEvent(int RepairOrderId, RepairOrderChangeType ChangeType);
}
