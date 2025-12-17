namespace Shared.Contracts.Events;

/// <summary>
/// Integration event published when an invoice is created in Billing context.
/// Can be consumed by OrderTaking or Shipping contexts to track billing status.
/// </summary>
public record InvoiceCreatedEvent(
    Guid MessageId,
    Guid CorrelationId,
    DateTime OccurredAt,
    int Version,
    
    Guid InvoiceId,
    Guid OrderId,
    decimal Amount,
    string InvoiceStatus
) : IIntegrationEvent
{
    public int Version { get; init; } = Version;
}

/// <summary>
/// Integration event published when an invoice is marked as paid in Billing context.
/// </summary>
public record InvoicePaidEvent(
    Guid MessageId,
    Guid CorrelationId,
    DateTime OccurredAt,
    int Version,
    
    Guid InvoiceId,
    Guid OrderId,
    decimal Amount,
    DateTime PaidAt
) : IIntegrationEvent
{
    public int Version { get; init; } = Version;
}

