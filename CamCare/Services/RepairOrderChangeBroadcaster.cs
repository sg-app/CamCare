using System.Collections.Concurrent;
using CamCare.Interfaces.Services;
using CamCare.Models;

namespace CamCare.Services
{
    /// <summary>
    /// Singleton-Implementierung des RepairOrder-Change-Bus.
    /// Hält die registrierten Subscriber (ein scoped Subscriber pro Blazor-Circuit) und verteilt
    /// Events im aktuellen Prozess. Für horizontale Skalierung kann diese Implementierung durch eine
    /// Redis-Backplane-Variante ersetzt werden, ohne dass Publisher oder Subscriber geändert werden.
    /// </summary>
    public sealed class RepairOrderChangeBroadcaster : IRepairOrderChangeBroadcaster
    {
        private readonly ConcurrentDictionary<Guid, IRepairOrderChangeSubscriber> _subscribers = new();
        private readonly ILogger<RepairOrderChangeBroadcaster> _logger;

        public RepairOrderChangeBroadcaster(ILogger<RepairOrderChangeBroadcaster> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(RepairOrderChangedEvent changeEvent, CancellationToken cancellationToken = default)
        {
            var subscribers = _subscribers.Values.ToArray();
            if (subscribers.Length == 0)
                return Task.CompletedTask;

            _logger.LogDebug(
                "Publiziere RepairOrder-Änderung ({ChangeType}) für Auftrag {RepairOrderId} an {SubscriberCount} Subscriber(s).",
                changeEvent.ChangeType, changeEvent.RepairOrderId, subscribers.Length);

            foreach (var subscriber in subscribers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    subscriber.Raise(changeEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Fehler beim Zustellen der RepairOrder-Änderung an einen Subscriber ({SubscriberId}).",
                        subscriber.Id);
                }
            }

            return Task.CompletedTask;
        }

        public void Register(IRepairOrderChangeSubscriber subscriber)
        {
            if (_subscribers.TryAdd(subscriber.Id, subscriber))
            {
                _logger.LogDebug("RepairOrder-Change-Subscriber {SubscriberId} registriert ({SubscriberCount} aktiv).",
                    subscriber.Id, _subscribers.Count);
            }
        }

        public void Unregister(IRepairOrderChangeSubscriber subscriber)
        {
            if (_subscribers.TryRemove(subscriber.Id, out _))
            {
                _logger.LogDebug("RepairOrder-Change-Subscriber {SubscriberId} abgemeldet ({SubscriberCount} aktiv).",
                    subscriber.Id, _subscribers.Count);
            }
        }
    }
}
