using CamCare.Interfaces.Services;
using CamCare.Models;

namespace CamCare.Services
{
    /// <summary>
    /// Scoped pro Blazor-Circuit. Registriert sich im Konstruktor beim Singleton-Broadcaster und
    /// meldet sich beim Ende des Circuits (Scope-Dispose) wieder ab - dadurch keine Memory-Leaks.
    /// Komponenten (z.&nbsp;B. die RepairOrders-Seite) abonnieren das <see cref="Changed"/>-Event.
    /// </summary>
    public sealed class RepairOrderChangeSubscriber : IRepairOrderChangeSubscriber
    {
        private readonly IRepairOrderChangeBroadcaster _broadcaster;
        private bool _disposed;

        public Guid Id { get; } = Guid.NewGuid();

        public event EventHandler<RepairOrderChangedEvent>? Changed;

        public RepairOrderChangeSubscriber(IRepairOrderChangeBroadcaster broadcaster)
        {
            _broadcaster = broadcaster;
            _broadcaster.Register(this);
        }

        public void Raise(RepairOrderChangedEvent changeEvent)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            Changed?.Invoke(this, changeEvent);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            Changed = null;
            _broadcaster.Unregister(this);
        }
    }
}
