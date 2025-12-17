namespace Shared.Infrastructure.Messaging;

/// <summary>
/// Base interface for all domain events.
/// Domain events represent significant business occurrences within a bounded context.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier for this event instance (for deduplication)
    /// </summary>
    Guid MessageId { get; }

    /// <summary>
    /// Correlation ID to track related events across the system
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Timestamp when the event occurred
    /// </summary>
    DateTime OccurredAt { get; }

    /// <summary>
    /// Version of the event contract (for versioning support)
    /// </summary>
    int Version { get; }
}

/// <summary>
/// Base interface for integration events that cross bounded context boundaries.
/// These are published to the message bus for other contexts to consume.
/// </summary>
public interface IIntegrationEvent : IDomainEvent
{
}

