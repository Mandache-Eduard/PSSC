namespace Billing.Domain.Repositories;

using Billing.Domain.Aggregates;
using Billing.Domain.ValueObjects;

/// <summary>
/// Repository interface for Invoice aggregate.
/// Domain does not depend on EF Core - this is the abstraction that infrastructure implements.
/// </summary>
public interface IInvoiceRepository
{
    /// <summary>
    /// Adds a new invoice to the repository
    /// </summary>
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an invoice by its ID
    /// </summary>
    Task<Invoice?> GetByIdAsync(InvoiceId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an invoice by the order it was created from
    /// </summary>
    Task<Invoice?> GetByOrderIdAsync(OrderId orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing invoice
    /// </summary>
    Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all invoices with a specific status
    /// </summary>
    Task<IReadOnlyList<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default);
}

