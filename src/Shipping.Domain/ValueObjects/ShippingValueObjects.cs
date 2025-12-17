namespace Shipping.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a shipment (owned by Shipping context)
/// </summary>
public sealed record ShipmentId(Guid Value)
{
    public static ShipmentId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

/// <summary>
/// Strongly-typed identifier for an order (reference only - Shipping doesn't own orders)
/// </summary>
public sealed record OrderId(Guid Value)
{
    public override string ToString() => Value.ToString();
}

/// <summary>
/// Represents an address in the Shipping context
/// </summary>
public sealed record Address(
    string Street,
    string City,
    string StateOrProvince,
    string PostalCode,
    string CountryCode)
{
    // Validation in constructor
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
/// Enum for shipment status in the shipping workflow
/// </summary>
public enum ShipmentStatus
{
    Created,        // Shipment created but not yet dispatched
    ReadyToShip,    // Shipment ready to dispatch
    Dispatched,     // Shipment dispatched from warehouse
    InTransit,      // Shipment in transit
    Delivered,      // Shipment delivered to customer
    Failed,         // Delivery failed
    Cancelled       // Shipment cancelled
}

