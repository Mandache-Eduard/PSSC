namespace Billing.Domain.Aggregates;

using Billing.Domain.ValueObjects;
using Shared.Infrastructure.Messaging;

/// <summary>
/// Invoice Aggregate Root - represents an invoice in the Billing bounded context.
/// Invoices are created in response to OrderPlaced events from the OrderTaking context.
/// This context DOES NOT own orders - it only references OrderId.
/// </summary>
public sealed class Invoice
{
    // Private constructor for EF Core
    private Invoice() { }

    /// <summary>
    /// Creates a new invoice from an order
    /// Note: This is typically called by a workflow/application service when handling OrderPlaced event
    /// </summary>
    public static Invoice CreateFromOrder(
        InvoiceId invoiceId,
        OrderId orderId,
        decimal totalAmount,
        string currencyCode = "USD")
    {
        if (totalAmount < 0)
            throw new ArgumentException("Total amount cannot be negative");

        return new Invoice
        {
            Id = invoiceId,
            OrderId = orderId,
            Amount = new Money(totalAmount, currencyCode),
            Status = InvoiceStatus.Created,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // ========== Properties ==========

    public InvoiceId Id { get; private set; } = null!;
    public OrderId OrderId { get; private set; } = null!;
    public Money Amount { get; private set; } = null!;
    public InvoiceStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    // ========== Methods ==========

    /// <summary>
    /// Marks the invoice as sent to the customer
    /// </summary>
    public void MarkAsSent()
    {
        if (Status != InvoiceStatus.Created)
            throw new InvalidOperationException($"Cannot send invoice in status '{Status}'");

        Status = InvoiceStatus.Sent;
        SentAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the invoice as paid by the customer
    /// </summary>
    public void MarkAsPaid()
    {
        if (Status is not (InvoiceStatus.Sent or InvoiceStatus.Overdue))
            throw new InvalidOperationException($"Cannot mark invoice in status '{Status}' as paid");

        Status = InvoiceStatus.Paid;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the invoice (e.g., when order is cancelled)
    /// </summary>
    public void Cancel()
    {
        if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException($"Cannot cancel invoice in status '{Status}'");

        Status = InvoiceStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the invoice as overdue if payment is not received by due date
    /// </summary>
    public void MarkAsOverdue()
    {
        if (Status is not (InvoiceStatus.Sent))
            throw new InvalidOperationException($"Cannot mark invoice in status '{Status}' as overdue");

        Status = InvoiceStatus.Overdue;
        UpdatedAt = DateTime.UtcNow;
    }
}

