namespace Billing.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for an invoice (owned by Billing context)
/// </summary>
public sealed record InvoiceId(Guid Value)
{
    public static InvoiceId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

/// <summary>
/// Strongly-typed identifier for an order (reference only - Billing doesn't own orders)
/// </summary>
public sealed record OrderId(Guid Value)
{
    public override string ToString() => Value.ToString();
}

/// <summary>
/// Represents a money value in the Billing context
/// </summary>
public sealed record Money(decimal Amount, string CurrencyCode = "USD")
{
    // Validation in constructor
    public Money(decimal amount, string currencyCode = "USD")
        : this(
            amount >= 0 ? amount : throw new ArgumentException("Amount must be non-negative"),
            currencyCode)
    {
    }

    public Money Add(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new InvalidOperationException($"Cannot add different currencies");
        return new Money(Amount + other.Amount, CurrencyCode);
    }

    public Money Subtract(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new InvalidOperationException($"Cannot subtract different currencies");
        return new Money(Amount - other.Amount, CurrencyCode);
    }

    public override string ToString() => $"{Amount:N2} {CurrencyCode}";
}

/// <summary>
/// Enum for invoice status in the billing workflow
/// </summary>
public enum InvoiceStatus
{
    Created,        // Invoice created but not yet sent
    Sent,           // Invoice sent to customer
    Paid,           // Invoice paid
    Overdue,        // Payment overdue
    Cancelled       // Invoice cancelled
}

