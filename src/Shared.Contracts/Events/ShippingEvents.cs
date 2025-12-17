namespace Shared.Contracts.Events;

/// <summary>
/// Integration event published when a shipment is created in Shipping context.
/// Can be consumed by OrderTaking or Billing contexts to track fulfillment status.
/// </summary>
public record ShipmentCreatedEvent(
    Guid MessageId,
    Guid CorrelationId,
    DateTime OccurredAt,
    int Version,
    
    Guid ShipmentId,
    Guid OrderId,
    string ShipmentStatus,
    DateTime? EstimatedDeliveryDate
) : IIntegrationEvent
{
    public int Version { get; init; } = Version;
}

/// <summary>
/// Integration event published when a shipment is dispatched in Shipping context.
/// </summary>
public record ShipmentDispatchedEvent(
    Guid MessageId,
    Guid CorrelationId,
    DateTime OccurredAt,
    int Version,
    
    Guid ShipmentId,
    Guid OrderId,
    string TrackingNumber,
    DateTime DispatchedAt
) : IIntegrationEvent
{
    public int Version { get; init; } = Version;
}

/// <summary>
/// Integration event published when a shipment is delivered in Shipping context.
/// </summary>
public record ShipmentDeliveredEvent(
    Guid MessageId,
    Guid CorrelationId,
    DateTime OccurredAt,
    int Version,
    
    Guid ShipmentId,
    Guid OrderId,
    DateTime DeliveredAt
) : IIntegrationEvent
{
    public int Version { get; init; } = Version;
}

