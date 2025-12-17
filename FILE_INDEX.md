# Order Management System - File Index

## 📚 Documentation Files

| File | Purpose |
|------|---------|
| `README.md` | Comprehensive architecture guide, API endpoints, deployment instructions |
| `QUICKSTART.md` | Quick reference, setup instructions, next steps |
| `IMPLEMENTATION_STATUS.md` | Detailed deliverables checklist, current phase status |

## 🏗️ Solution Structure

### OrderTaking Bounded Context

**Domain Layer** (`src/OrderTaking.Domain/`)
- `Aggregates/Order.cs` - Order aggregate root with state machine
- `ValueObjects/OrderValueObjects.cs` - OrderId, CustomerId, Money, Address, OrderLineItem, OrderStatus
- `Repositories/IOrderRepository.cs` - Repository interface for persistence

**Infrastructure Layer** (`src/OrderTaking.Infrastructure/`)
- *To be implemented*: OrderDbContext, OrderRepository implementation, migrations

**Application Layer** (`src/OrderTaking.Application/`)
- *To be implemented*: PlaceOrderWorkflow, ConfirmOrderWorkflow, CancelOrderWorkflow

### Billing Bounded Context

**Domain Layer** (`src/Billing.Domain/`)
- `Aggregates/Invoice.cs` - Invoice aggregate root for billing
- `ValueObjects/BillingValueObjects.cs` - InvoiceId, OrderId, Money, InvoiceStatus
- `Repositories/IInvoiceRepository.cs` - Repository interface

**Infrastructure Layer** (`src/Billing.Infrastructure/`)
- *To be implemented*: BillingDbContext, InvoiceRepository, migrations

**Application Layer** (`src/Billing.Application/`)
- *To be implemented*: CreateInvoiceFromOrderWorkflow, MarkInvoiceAsPaidWorkflow, consumers

### Shipping Bounded Context

**Domain Layer** (`src/Shipping.Domain/`)
- `Aggregates/Shipment.cs` - Shipment aggregate for fulfillment
- `ValueObjects/ShippingValueObjects.cs` - ShipmentId, OrderId, Address, ShipmentStatus
- `Repositories/IShipmentRepository.cs` - Repository interface

**Infrastructure Layer** (`src/Shipping.Infrastructure/`)
- *To be implemented*: ShippingDbContext, ShipmentRepository, migrations

**Application Layer** (`src/Shipping.Application/`)
- *To be implemented*: CreateShipmentFromOrderWorkflow, DispatchShipmentWorkflow, consumers

### Shared Components

**Contracts** (`src/Shared.Contracts/`)
- `Events/OrderEvents.cs` - OrderPlaced, OrderCanceled events
- `Events/BillingEvents.cs` - InvoiceCreated, InvoicePaid events
- `Events/ShippingEvents.cs` - ShipmentCreated, ShipmentDispatched, ShipmentDelivered events

**Infrastructure** (`src/Shared.Infrastructure/`)
- `Results/Result.cs` - Result<TSuccess, TError> type for typed error handling
- `Messaging/IDomainEvent.cs` - Event interfaces
- `Messaging/IEventBus.cs` - Event bus abstraction
- `Messaging/IOutboxAndInbox.cs` - Outbox/Inbox pattern interfaces

### API Layer

**Api** (`src/Api/`)
- *To be implemented*: 
  - `Controllers/OrdersController.cs` - REST endpoints for orders
  - `Controllers/InvoicesController.cs` - REST endpoints for invoices
  - `Controllers/ShipmentsController.cs` - REST endpoints for shipments
  - `Program.cs` - ASP.NET Core configuration, dependency injection, messaging setup

### Tests

**OrderTaking Tests** (`tests/OrderTaking.Tests/`)
- *To be implemented*: Domain model tests, workflow tests, integration tests

**Billing Tests** (`tests/Billing.Tests/`)
- *To be implemented*: Domain model tests, workflow tests, event consumer tests

**Shipping Tests** (`tests/Shipping.Tests/`)
- *To be implemented*: Domain model tests, workflow tests, event consumer tests

## 🔑 Key Types & Interfaces

### Result Type (Shared.Infrastructure)
```csharp
Result<TSuccess, TError>
  - Success { Value: TSuccess }
  - Failure { Error: TError }
  - Match<T>(onSuccess, onFailure) → T
  - Map<TNew>(transform) → Result<TNew, TError>
  - Bind<TNew>(chain) → Result<TNew, TError>
```

### Domain Events
```csharp
IDomainEvent
  - MessageId: Guid
  - CorrelationId: Guid
  - OccurredAt: DateTime
  - Version: int

IIntegrationEvent : IDomainEvent
```

### Repository Interfaces

**IOrderRepository**
```csharp
AddAsync(Order) → Task
GetByIdAsync(OrderId) → Task<Order?>
GetByCustomerIdAsync(CustomerId) → Task<IReadOnlyList<Order>>
UpdateAsync(Order) → Task
GetByStatusAsync(OrderStatus) → Task<IReadOnlyList<Order>>
```

**IInvoiceRepository**
```csharp
AddAsync(Invoice) → Task
GetByIdAsync(InvoiceId) → Task<Invoice?>
GetByOrderIdAsync(OrderId) → Task<Invoice?>
UpdateAsync(Invoice) → Task
GetByStatusAsync(InvoiceStatus) → Task<IReadOnlyList<Invoice>>
```

**IShipmentRepository**
```csharp
AddAsync(Shipment) → Task
GetByIdAsync(ShipmentId) → Task<Shipment?>
GetByOrderIdAsync(OrderId) → Task<Shipment?>
UpdateAsync(Shipment) → Task
GetByStatusAsync(ShipmentStatus) → Task<IReadOnlyList<Shipment>>
```

## 📊 Aggregate State Machines

### Order (OrderTaking)
```
        ┌─ Pending
        │    ├─ confirm() → Confirmed
        │    ├─ cancel() → Cancelled
        │    └─ addLineItem() → Pending (same)
        ↓
    Confirmed
        ├─ cancel() → Cancelled
        └─ markAsCompleted() → Completed
        ↓
    Completed (final state)
        
    Cancelled (final state)
```

### Invoice (Billing)
```
    Created
        ├─ markAsSent() → Sent
        └─ cancel() → Cancelled
        ↓
    Sent
        ├─ markAsPaid() → Paid
        ├─ markAsOverdue() → Overdue
        └─ cancel() → Cancelled
        ↓
    Overdue
        └─ markAsPaid() → Paid
        ↓
    Paid (final state)
    
    Cancelled (final state)
```

### Shipment (Shipping)
```
    Created
        ├─ markAsReadyToShip() → ReadyToShip
        └─ cancel() → Cancelled
        ↓
    ReadyToShip
        ├─ markAsDispatched(tracking) → Dispatched
        └─ cancel() → Cancelled
        ↓
    Dispatched
        ├─ markAsInTransit() → InTransit
        ├─ markAsFailed() → Failed
        └─ cancel() → Cancelled
        ↓
    InTransit
        ├─ markAsDelivered() → Delivered
        └─ markAsFailed() → Failed
        ↓
    Delivered (final state)
    
    Failed → (can be retried)
    Cancelled (final state)
```

## 🔌 Integration Events Flow

```
OrderTaking Context
    │
    ├─ PlaceOrder Workflow
    │   └─ Emits: OrderPlaced Event
    │       ├─ → Billing Consumer (CreateInvoiceFromOrder)
    │       │    └─ Emits: InvoiceCreated Event
    │       │
    │       └─ → Shipping Consumer (CreateShipmentFromOrder)
    │            └─ Emits: ShipmentCreated Event
    │
    └─ CancelOrder Workflow
        └─ Emits: OrderCanceled Event
            ├─ → Billing Consumer
            │    └─ Cancels Invoice
            │
            └─ → Shipping Consumer
                 └─ Cancels Shipment
```

## 📝 Database Schema (Planned)

### OrderManagementSystem_OrderTaking
- Orders (OrderId, CustomerId, Status, ShippingAddress, BillingAddress, TotalAmount, CreatedAt, UpdatedAt)
- OrderLineItems (OrderLineItemId, OrderId, ProductId, ProductName, Quantity, UnitPrice, LineTotal)
- Outbox (MessageId, EventType, Payload, PublishedAt)
- Inbox (MessageId, EventType, ProcessedAt)

### OrderManagementSystem_Billing
- Invoices (InvoiceId, OrderId, Amount, CurrencyCode, Status, SentAt, PaidAt, CancelledAt, CreatedAt, UpdatedAt)
- Outbox (MessageId, EventType, Payload, PublishedAt)
- Inbox (MessageId, EventType, ProcessedAt)

### OrderManagementSystem_Shipping
- Shipments (ShipmentId, OrderId, ShippingAddress, Status, TrackingNumber, EstimatedDeliveryDate, DispatchedAt, DeliveredAt, CancelledAt, CreatedAt, UpdatedAt)
- Outbox (MessageId, EventType, Payload, PublishedAt)
- Inbox (MessageId, EventType, ProcessedAt)

## 🚀 Deployment Architecture (Planned)

```
┌─────────────────────────────────────────────────────────┐
│                   API Gateway / Load Balancer           │
└────────────────┬──────────────────────────┬─────────────┘
                 │                          │
        ┌────────▼──────────┐        ┌──────▼──────────┐
        │  OrderTaking.Api  │        │  Single API     │
        │  - PlaceOrder     │◄──────►│  (ASP.NET Core) │
        │  - ConfirmOrder   │        │                 │
        │  - CancelOrder    │        │  Routes to:     │
        └────────┬──────────┘        │  - OrderTaking  │
                 │                   │  - Billing      │
        ┌────────▼──────────┐        │  - Shipping     │
        │  OrderTaking DB   │        └──────┬──────────┘
        │  (LocalDB)        │               │
        └───────────────────┘    ┌──────────▼───────────┐
                                 │   RabbitMQ / MQ Bus  │
                                 │  (Dev: In-Memory)    │
                                 └──────────┬───────────┘
                                            │
                    ┌───────────────────────┼────────────────────┐
                    │                       │                    │
        ┌───────────▼──────────┐  ┌────────▼────────┐  ┌───────▼────────┐
        │   Billing Service    │  │ Shipping Service│  │  Analytics     │
        │  - Consumer Handler  │  │ - Consumer      │  │  - Event Log   │
        │  - CreateInvoice     │  │   Handler       │  │  - Dashboard   │
        │  - Invoice DB        │  │ - CreateShipment│  └────────────────┘
        └──────────────────────┘  │ - Shipment DB   │
                                 └─────────────────┘
```

## 🔐 Security Considerations (Planned)

- API key authentication for endpoints
- Correlation ID tracking for audit logs
- Message signing/encryption for RabbitMQ
- Database role-based access control
- Input validation at API boundaries
- Rate limiting on public endpoints

## 📈 Performance Considerations (Planned)

- Database indexes on frequently queried columns (OrderId, OrderStatus, InvoiceStatus, etc.)
- Pagination for list endpoints
- Caching layer for read models
- Batch publishing in Outbox worker
- Connection pooling for database
- Message batching in consumers

## 📚 References & Learning Resources

- **Domain-Driven Design**: Eric Evans' "Domain-Driven Design" book
- **MassTransit**: https://masstransit.io/
- **Entity Framework Core**: https://docs.microsoft.com/en-us/ef/core/
- **Outbox Pattern**: https://www.microsoft.com/en-us/research/publication/the-outbox-pattern/
- **Inbox Pattern**: https://jeremydmiller.com/2021/05/11/async-await-and-the-outbox-pattern/
- **.NET Best Practices**: https://docs.microsoft.com/en-us/dotnet/

---

**Last Updated**: December 17, 2025  
**Phase**: Scaffolding & Domain Models Complete | Infrastructure & Workflows Next

