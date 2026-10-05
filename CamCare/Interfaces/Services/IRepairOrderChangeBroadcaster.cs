using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Einfacher In-Process-Pub/Sub-Bus für RepairOrder-Änderungen.
    /// Publisher (z.&nbsp;B. <see cref="IRepairOrderService"/>) rufen <see cref="PublishAsync"/> auf;
    /// registrierte Subscriber (ein scoped <see cref="IRepairOrderChangeSubscriber"/> pro Blazor-Circuit)
    /// werden über ein Event benachrichtigt.
    /// <para>
    /// Die Implementierung ist Singleton und prozessintern. Für horizontale Skalierung (mehrere
    /// Serverinstanzen) kann sie später durch eine verteilte Variante (z.&nbsp;B. Redis-Backplane oder
    /// SignalR-Hub) ersetzt werden, ohne dass Publisher oder Subscriber angepasst werden müssen.
    /// </para>
    /// </summary>
    public interface IRepairOrderChangeBroadcaster
    {
        Task PublishAsync(RepairOrderChangedEvent changeEvent, CancellationToken cancellationToken = default);

        void Register(IRepairOrderChangeSubscriber subscriber);

        void Unregister(IRepairOrderChangeSubscriber subscriber);
    }
}
