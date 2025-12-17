namespace Shipping.Domain.Aggregates;

using Shipping.Domain.ValueObjects;
using Shared.Infrastructure.Messaging;

/// <summary>
/// Shipment Aggregate Root - represents a shipment in the Shipping bounded context.
/// Shipments are created in response to OrderPlaced events from the OrderTaking context.
/// This context DOES NOT own orders - it only references OrderId and shipping address.
/// </summary>
public sealed class Shipment
{
    // Private constructor for EF Core
    private Shipment() { }

    /// <summary>
    /// Creates a new shipment from an order
    /// Note: This is typically called by a workflow/application service when handling OrderPlaced event
    /// </summary>
    public static Shipment CreateFromOrder(
        ShipmentId shipmentId,
        OrderId orderId,
        Address shippingAddress,
        DateTime? estimatedDeliveryDate = null)
    {
        if (shippingAddress == null)
            throw new ArgumentNullException(nameof(shippingAddress));

        return new Shipment
        {
            Id = shipmentId,
            OrderId = orderId,
            ShippingAddress = shippingAddress,
            Status = ShipmentStatus.Created,
            EstimatedDeliveryDate = estimatedDeliveryDate ?? DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // ========== Properties ==========

    public ShipmentId Id { get; private set; } = null!;
    public OrderId OrderId { get; private set; } = null!;
    public Address ShippingAddress { get; private set; } = null!;
    public ShipmentStatus Status { get; private set; }
    public string? TrackingNumber { get; private set; }
    public DateTime EstimatedDeliveryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DispatchedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    // ========== Methods ==========

    /// <summary>
    /// Marks the shipment as ready to ship from the warehouse
    /// </summary>
    public void MarkAsReadyToShip()
    {
        if (Status != ShipmentStatus.Created)
            throw new InvalidOperationException($"Cannot mark shipment in status '{Status}' as ready to ship");

        Status = ShipmentStatus.ReadyToShip;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the shipment as dispatched with a tracking number
    /// </summary>
    public void MarkAsDispatched(string trackingNumber)
    {
        if (Status != ShipmentStatus.ReadyToShip)
            throw new InvalidOperationException($"Cannot dispatch shipment in status '{Status}'");

        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Tracking number is required");

        Status = ShipmentStatus.Dispatched;
        TrackingNumber = trackingNumber;
        DispatchedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the shipment as in transit to customer
    /// </summary>
    public void MarkAsInTransit()
    {
        if (Status != ShipmentStatus.Dispatched)
            throw new InvalidOperationException($"Cannot mark shipment in status '{Status}' as in transit");

        Status = ShipmentStatus.InTransit;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the shipment as delivered to customer
    /// </summary>
    public void MarkAsDelivered()
    {
        if (Status != ShipmentStatus.InTransit)
            throw new InvalidOperationException($"Cannot deliver shipment in status '{Status}'");

        Status = ShipmentStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the shipment as failed (delivery attempt failed)
    /// </summary>
    public void MarkAsFailed()
    {
        if (Status is ShipmentStatus.Delivered or ShipmentStatus.Failed or ShipmentStatus.Cancelled)
            throw new InvalidOperationException($"Cannot mark shipment in status '{Status}' as failed");

        Status = ShipmentStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the shipment (e.g., when order is cancelled)
    /// </summary>
    public void Cancel()
    {
        if (Status is ShipmentStatus.Delivered or ShipmentStatus.Cancelled)
            throw new InvalidOperationException($"Cannot cancel shipment in status '{Status}'");

        Status = ShipmentStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}

