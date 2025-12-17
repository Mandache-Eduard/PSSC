namespace OrderTaking.Domain.Aggregates;

using OrderTaking.Domain.ValueObjects;
using Shared.Infrastructure.Messaging;

/// <summary>
/// Order Aggregate Root - represents the core order entity in the OrderTaking bounded context.
/// This is the primary entity owning order data within this context.
/// Other contexts (Billing, Shipping) do NOT have access to the Order - they only react to OrderPlaced/OrderCanceled events.
/// </summary>
public sealed class Order
{
    private readonly List<OrderLineItem> _lineItems = [];

    // Private constructor for EF Core
    private Order() { }

    /// <summary>
    /// Creates a new order with initial line items
    /// </summary>
    public static Order Create(
        OrderId orderId,
        CustomerId customerId,
        List<OrderLineItem> lineItems,
        Address shippingAddress,
        Address billingAddress)
    {
        if (lineItems == null || lineItems.Count == 0)
            throw new ArgumentException("Order must have at least one line item");

        var order = new Order
        {
            Id = orderId,
            CustomerId = customerId,
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        order._lineItems.AddRange(lineItems);
        return order;
    }

    // ========== Properties ==========

    public OrderId Id { get; private set; } = null!;
    public CustomerId CustomerId { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;
    public Address BillingAddress { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Read-only collection of line items
    public IReadOnlyList<OrderLineItem> LineItems => _lineItems.AsReadOnly();

    // ========== Methods ==========

    /// <summary>
    /// Confirms the order and makes it ready for billing and shipping workflows
    /// </summary>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm order in status '{Status}'");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the order (only allowed in certain states)
    /// </summary>
    public void Cancel()
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException($"Cannot cancel order in status '{Status}'");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a line item to the order (only allowed if not yet confirmed)
    /// </summary>
    public void AddLineItem(OrderLineItem lineItem)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot add line items to order in status '{Status}'");

        _lineItems.Add(lineItem);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the total amount for the order
    /// </summary>
    public Money GetTotalAmount()
    {
        if (_lineItems.Count == 0)
            return new Money(0, "USD");

        var total = _lineItems.Aggregate(
            new Money(0, _lineItems[0].UnitPrice.CurrencyCode),
            (acc, item) => acc.Add(item.GetLineTotal())
        );

        return total;
    }

    /// <summary>
    /// Marks the order as completed (for workflow orchestration)
    /// </summary>
    public void MarkAsCompleted()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException($"Cannot complete order in status '{Status}'");

        Status = OrderStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
}

