namespace Shipping.Domain.Repositories;

using Shipping.Domain.Aggregates;
using Shipping.Domain.ValueObjects;

/// <summary>
/// Repository interface for Shipment aggregate.
/// Domain does not depend on EF Core - this is the abstraction that infrastructure implements.
/// </summary>
public interface IShipmentRepository
{
    /// <summary>
    /// Adds a new shipment to the repository
    /// </summary>
    Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a shipment by its ID
    /// </summary>
    Task<Shipment?> GetByIdAsync(ShipmentId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a shipment by the order it was created from
    /// </summary>
    Task<Shipment?> GetByOrderIdAsync(OrderId orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing shipment
    /// </summary>
    Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all shipments with a specific status
    /// </summary>
    Task<IReadOnlyList<Shipment>> GetByStatusAsync(ShipmentStatus status, CancellationToken cancellationToken = default);
}

