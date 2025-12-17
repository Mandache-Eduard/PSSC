namespace Shared.Contracts.Events;

/// <summary>
/// Integration event published when an order is successfully placed in OrderTaking context.
/// Consumed by Billing and Shipping contexts to create invoices and shipments.
/// </summary>
public record OrderPlacedEvent(
    Guid MessageId,
    Guid CorrelationId,
    DateTime OccurredAt,
    int Version,
    
    // Order details
    Guid OrderId,
    Guid CustomerId,
    List<OrderLineDto> OrderLines,
    decimal TotalAmount,
    string Status
) : IIntegrationEvent
{
    public int Version { get; init; } = Version;
}

/// <summary>
/// Represents a single line item in an order
/// </summary>
public record OrderLineDto(
    string ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);

/// <summary>
/// Integration event published when an order is successfully canceled in OrderTaking context.
/// Consumed by Billing and Shipping contexts to cancel invoices and shipments.
/// </summary>
public record OrderCanceledEvent(
    Guid MessageId,
    Guid CorrelationId,
    DateTime OccurredAt,
    int Version,
    
    Guid OrderId,
    string CancellationReason
) : IIntegrationEvent
{
    public int Version { get; init; } = Version;
}

/// <summary>
/// Marker interface to indicate these are integration events
/// </summary>
internal interface IIntegrationEvent
{
    Guid MessageId { get; }
    Guid CorrelationId { get; }
    DateTime OccurredAt { get; }
    int Version { get; }
}

