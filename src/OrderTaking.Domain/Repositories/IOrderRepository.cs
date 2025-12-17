namespace OrderTaking.Domain.Repositories;

using OrderTaking.Domain.Aggregates;
using OrderTaking.Domain.ValueObjects;

/// <summary>
/// Repository interface for Order aggregate.
/// Domain does not depend on EF Core - this is the abstraction that infrastructure implements.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Adds a new order to the repository
    /// </summary>
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an order by its ID
    /// </summary>
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all orders for a customer
    /// </summary>
    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing order
    /// </summary>
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all orders with a specific status
    /// </summary>
    Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
}

