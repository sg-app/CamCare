using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Scoped pro Blazor-Circuit. Registriert sich beim <see cref="IRepairOrderChangeBroadcaster"/>
    /// und stellt RepairOrder-Änderungen an angeschlossene UI-Komponenten zu.
    /// Die Lebensdauer entspricht dem Circuit; beim Scope-Dispose wird die Registrierung entfernt.
    /// </summary>
    public interface IRepairOrderChangeSubscriber : IDisposable
    {
        Guid Id { get; }

        event EventHandler<RepairOrderChangedEvent>? Changed;

        void Raise(RepairOrderChangedEvent changeEvent);
    }
}
