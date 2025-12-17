# Quick Start Guide

## Project Structure Summary

This Order Management System is organized into **3 independent bounded contexts** using Domain-Driven Design principles:

### Bounded Contexts

1. **OrderTaking** - Manages order capture and state
2. **Billing** - Handles invoicing and payments
3. **Shipping** - Coordinates fulfillment and delivery

Each context has:
- `.Domain` - Pure domain logic (no frameworks)
- `.Infrastructure` - EF Core, repositories, migrations
- `.Application` - Workflows, command handlers

### Shared Components

- **Shared.Contracts** - Integration event definitions (no business logic)
- **Shared.Infrastructure** - Messaging abstractions, Result<T> type, base interfaces
- **Api** - Central ASP.NET Core API entry point

### Tests

- `OrderTaking.Tests/` - Domain and workflow tests
- `Billing.Tests/` - Domain and workflow tests  
- `Shipping.Tests/` - Domain and workflow tests

## Building the Solution

```bash
# Restore packages
dotnet restore

# Build all projects
dotnet build

# Build specific context
dotnet build src/OrderTaking.Domain
dotnet build src/Billing.Infrastructure
dotnet build src/Shipping.Application
```

## Domain Models Implemented ✓

### OrderTaking
- **Order** aggregate with state machine (Pending → Confirmed → Completed/Cancelled)
- **OrderId**, **CustomerId**, **Money**, **Address**, **OrderLineItem** value objects
- **IOrderRepository** interface for persistence

### Billing
- **Invoice** aggregate with billing state (Created → Sent → Paid/Overdue/Cancelled)
- **InvoiceId**, **OrderId**, **Money** value objects
- **IInvoiceRepository** interface for persistence

### Shipping
- **Shipment** aggregate with fulfillment states (Created → ReadyToShip → Dispatched → InTransit → Delivered)
- **ShipmentId**, **OrderId**, **Address** value objects
- **IShipmentRepository** interface for persistence

## Integration Events ✓

Contexts communicate via integration events:

- **OrderPlaced** → Triggers invoice and shipment creation
- **OrderCanceled** → Triggers cancellations in downstream contexts
- **InvoiceCreated** → Billing context publishes
- **ShipmentCreated** → Shipping context publishes
- **ShipmentDispatched** → Shipping context publishes
- **ShipmentDelivered** → Shipping context publishes

All events include: **MessageId**, **CorrelationId**, **OccurredAt**, **Version** for reliability and tracing.

## Typed Result Pattern ✓

All workflows use `Result<TSuccess, TError>` for typed error handling:

```csharp
public Result<Order, ValidationError> PlaceOrder(/* params */)
{
    // Validate inputs
    if (invalid) return new Result<Order, ValidationError>.Failure(error);
    
    // Create and save
    var order = Order.Create(/* params */);
    await _repository.AddAsync(order);
    
    return new Result<Order, ValidationError>.Success(order);
}

// Usage
var result = await workflowService.PlaceOrder(/* params */);
result.Match(
    onSuccess: order => { /* handle success */ },
    onFailure: error => { /* handle error */ }
);
```

## Repository Pattern ✓

Domain models depend on repository interfaces, not EF Core:

```csharp
// Domain layer (OrderTaking.Domain)
public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetByIdAsync(OrderId id);
    Task UpdateAsync(Order order);
    // ... more query methods
}

// Infrastructure layer (OrderTaking.Infrastructure) implements
public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;
    // ... implementation with EF Core
}
```

This allows:
- Easy testing with mock repositories
- Changing persistence layer without domain changes
- Clear separation of concerns

## Next Implementation Steps

### Phase 1: Infrastructure (EF Core)
1. Create DbContext per context
2. Implement repository classes
3. Configure entity mappings
4. Create migrations

### Phase 2: Workflows & Application Services
1. Implement workflow services for each context
2. Create command handlers
3. Integrate Result<T> pattern
4. Add idempotency/deduplication logic

### Phase 3: Messaging Integration
1. Setup MassTransit consumers
2. Implement Outbox pattern (reliability)
3. Implement Inbox pattern (idempotency)
4. Configure retry policies and DLQ

### Phase 4: API Layer
1. Create REST controllers
2. Map domain errors to HTTP responses
3. Implement query endpoints
4. Add Swagger/OpenAPI documentation

### Phase 5: Testing
1. Domain model unit tests
2. Workflow integration tests
3. API endpoint tests
4. Consumer/messaging tests

## Architecture Principles Applied

✅ **Domain-Driven Design**
- Bounded contexts with clear ownership
- Aggregates and value objects
- Repository pattern
- Domain invariants enforced

✅ **Clean Architecture**
- Domain layer has no framework dependencies
- Dependency inversion (interfaces)
- Organized by domain concern, not technical layer

✅ **Event-Driven Architecture**
- Contexts communicate via events
- Loose coupling between contexts
- Independent deployment possible

✅ **Typed Error Handling**
- Result<T> eliminates unexpected exceptions for business logic
- Type-safe error paths
- Better control flow and testability

✅ **Reliable Messaging**
- Outbox pattern ensures durability
- Inbox pattern ensures idempotency
- Correlation IDs for tracing
- DLQ for failed messages

## File Structure Summary

```
src/
├── OrderTaking.Domain/
│   ├── Aggregates/Order.cs
│   ├── ValueObjects/OrderValueObjects.cs
│   └── Repositories/IOrderRepository.cs
├── OrderTaking.Infrastructure/ (EF Core DbContext, migrations, impl)
├── OrderTaking.Application/    (Workflows, command handlers)
│
├── Billing.Domain/
│   ├── Aggregates/Invoice.cs
│   ├── ValueObjects/BillingValueObjects.cs
│   └── Repositories/IInvoiceRepository.cs
├── Billing.Infrastructure/
├── Billing.Application/
│
├── Shipping.Domain/
│   ├── Aggregates/Shipment.cs
│   ├── ValueObjects/ShippingValueObjects.cs
│   └── Repositories/IShipmentRepository.cs
├── Shipping.Infrastructure/
├── Shipping.Application/
│
├── Shared.Contracts/
│   └── Events/
│       ├── OrderEvents.cs
│       ├── BillingEvents.cs
│       └── ShippingEvents.cs
├── Shared.Infrastructure/
│   ├── Results/Result.cs
│   └── Messaging/
│       ├── IDomainEvent.cs
│       ├── IEventBus.cs
│       └── IOutboxAndInbox.cs
│
└── Api/
    ├── Program.cs
    ├── Controllers/ (to be implemented)
    └── appsettings.json
```

## Database Strategy

Each bounded context will have its own database:
- `OrderManagementSystem_OrderTaking`
- `OrderManagementSystem_Billing`
- `OrderManagementSystem_Shipping`
- `OrderManagementSystem_Shared` (Inbox/Outbox tables)

Using SQL Server LocalDB for local development:
```
Server=(localdb)\mssqllocaldb;Database=OrderManagementSystem_*;Integrated Security=true;
```

## Messaging Strategy

Inter-context communication via MassTransit:
- **Transport**: RabbitMQ (production) / In-Memory (development)
- **Reliability**: Outbox pattern in domain, Inbox pattern in consumers
- **Retry Policy**: 5s, 30s, 5min delays before DLQ
- **Idempotency**: Message ID stored in Inbox table

## Key Design Decisions

1. **Separate Value Objects Per Context** - Even though Order and Invoice both have Money/Address, each context defines its own to maintain independence

2. **Result<T> Pattern** - Avoids exception-based error handling for expected business errors, improving clarity and testability

3. **Repository Pattern** - Allows domain logic to remain framework-agnostic and testable

4. **Event-Driven Communication** - Contexts don't call each other directly; they publish events and react independently

5. **Inbox/Outbox** - Ensures at-least-once delivery and idempotent processing despite network/database failures

## Git Workflow

The repository includes meaningful commits showing collaboration:

```bash
# View commit history
git log --oneline

# See commits from each developer
git log --author="Developer One" --oneline
git log --author="Developer Two" --oneline
```

Suggested workflow for contributions:
```bash
# Create feature branch
git checkout -b feature/order-workflows

# Implement feature
# ... make changes ...

# Commit with clear messages
git commit -m "feat(order-taking): Implement PlaceOrder workflow

- Added workflow validation logic
- Integrated Result<T> for typed errors
- Added integration test coverage"

# Push to remote
git push origin feature/order-workflows

# Create pull request on GitHub
```

## Getting Help

Refer to:
- `README.md` - Full architecture and API documentation
- `IMPLEMENTATION_STATUS.md` - Current phase and next steps
- `QUICKSTART.md` - This file, quick reference guide

---

**Status**: ✅ Scaffolding Complete | ✅ Domain Models Complete | ⏳ Infrastructure/Workflows/API: Next Phase

Ready to start Phase 1 (Infrastructure implementation)! 🚀

