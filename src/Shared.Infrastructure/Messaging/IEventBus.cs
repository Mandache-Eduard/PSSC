namespace Shared.Infrastructure.Messaging;

/// <summary>
/// Interface for the message bus/event publisher used to dispatch integration events
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an integration event to the message bus
    /// </summary>
    Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes multiple integration events to the message bus
    /// </summary>
    Task PublishBatchAsync(IEnumerable<IIntegrationEvent> events, CancellationToken cancellationToken = default);
}

