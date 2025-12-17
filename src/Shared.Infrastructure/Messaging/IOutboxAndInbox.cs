namespace Shared.Infrastructure.Messaging;

/// <summary>
/// Interface for storing processed message IDs (Inbox pattern).
/// Used to ensure idempotent processing of messages - each message is processed at most once.
/// </summary>
public interface IInboxService
{
    /// <summary>
    /// Marks a message as processed. If already processed, returns false.
    /// </summary>
    /// <param name="messageId">Unique identifier of the message</param>
    /// <param name="messageType">Type name of the message</param>
    /// <returns>True if the message was newly processed; false if already processed</returns>
    Task<bool> MarkAsProcessedAsync(Guid messageId, string messageType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a message has already been processed
    /// </summary>
    Task<bool> IsProcessedAsync(Guid messageId, string messageType, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for storing outgoing events (Outbox pattern).
/// Ensures reliable publishing of events with transaction-like guarantees.
/// </summary>
public interface IOutboxService
{
    /// <summary>
    /// Adds an event to the outbox for later publishing
    /// </summary>
    Task AddAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves unpublished events from the outbox
    /// </summary>
    Task<IReadOnlyList<IIntegrationEvent>> GetUnpublishedAsync(int batchSize = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an event as published
    /// </summary>
    Task MarkAsPublishedAsync(Guid messageId, CancellationToken cancellationToken = default);
}

