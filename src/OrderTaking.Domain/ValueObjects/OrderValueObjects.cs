namespace OrderTaking.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for an order. Prevents mixing up order IDs with other GUIDs.
/// </summary>
public sealed record OrderId(Guid Value)
{
    /// <summary>
    /// Creates a new unique OrderId
    /// </summary>
    public static OrderId New() => new(Guid.NewGuid());

    /// <summary>
    /// Prevents accidental default values
    /// </summary>
    public override string ToString() => Value.ToString();
}

/// <summary>
/// Strongly-typed identifier for a customer
/// </summary>
public sealed record CustomerId(Guid Value)
{
    public static CustomerId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

/// <summary>
/// Represents money with currency. Prevents errors like adding dollars to euros.
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

    /// <summary>
    /// Adds two money values (must have same currency)
    /// </summary>
    public Money Add(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new InvalidOperationException($"Cannot add different currencies: {CurrencyCode} + {other.CurrencyCode}");
        return new Money(Amount + other.Amount, CurrencyCode);
    }

    /// <summary>
    /// Subtracts two money values
    /// </summary>
    public Money Subtract(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new InvalidOperationException($"Cannot subtract different currencies: {CurrencyCode} - {other.CurrencyCode}");
        return new Money(Amount - other.Amount, CurrencyCode);
    }

    public override string ToString() => $"{Amount:N2} {CurrencyCode}";
}

/// <summary>
/// Represents a physical address or billing address
/// </summary>
public sealed record Address(
    string Street,
    string City,
    string StateOrProvince,
    string PostalCode,
    string CountryCode)
{
    // Validation in constructor - call the positional record constructor with validated values
    public Address(
        string street,
        string city,
        string stateOrProvince,
        string postalCode,
        string countryCode)
        : this(
            !string.IsNullOrWhiteSpace(street) ? street : throw new ArgumentException("Street is required"),
            !string.IsNullOrWhiteSpace(city) ? city : throw new ArgumentException("City is required"),
            !string.IsNullOrWhiteSpace(stateOrProvince) ? stateOrProvince : throw new ArgumentException("State/Province is required"),
            !string.IsNullOrWhiteSpace(postalCode) ? postalCode : throw new ArgumentException("Postal code is required"),
            !string.IsNullOrWhiteSpace(countryCode) ? countryCode : throw new ArgumentException("Country code is required"))
    {
    }

    public override string ToString() => $"{Street}, {City}, {StateOrProvince} {PostalCode}, {CountryCode}";
}

/// <summary>
/// Represents an order line item (e.g., one product in an order)
/// </summary>
public sealed record OrderLineItem(
    string ProductId,
    string ProductName,
    int Quantity,
    Money UnitPrice)
{
    // Validation in constructor
    public OrderLineItem(
        string productId,
        string productName,
        int quantity,
        Money unitPrice)
        : this(
            !string.IsNullOrWhiteSpace(productId) ? productId : throw new ArgumentException("ProductId is required"),
            !string.IsNullOrWhiteSpace(productName) ? productName : throw new ArgumentException("ProductName is required"),
            quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive"),
            unitPrice)
    {
    }

    /// <summary>
    /// Calculates the total cost of this line item
    /// </summary>
    public Money GetLineTotal() => new(UnitPrice.Amount * Quantity, UnitPrice.CurrencyCode);

    public override string ToString() => $"{Quantity}x {ProductName} @ {UnitPrice}";
}

/// <summary>
/// Strongly-typed order status to prevent invalid state transitions
/// </summary>
public enum OrderStatus
{
    Pending,        // Order created but not yet confirmed
    Confirmed,      // Order confirmed and ready for billing/shipping
    Cancelled,      // Order cancelled
    Completed       // Order fully processed (delivered and paid)
}

