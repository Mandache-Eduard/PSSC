# Order Management System (DDD + Typed Workflows + Async Messaging)

A comprehensive Order Management system demonstrating Domain-Driven Design (DDD), typed workflows, and asynchronous inter-service messaging in C# .NET 9.

## Architecture Overview

### Three Bounded Contexts

1. **OrderTaking** - Handles order capture, modification, and cancellation
2. **Billing** - Manages invoicing and payment tracking
3. **Shipping** - Coordinates shipments, tracking, and delivery

Each context is strictly separated with:
- Own domain models (aggregates, value objects)
- Own database (SQL Server LocalDB)
- Own EF Core DbContext and migrations
- No direct cross-context database access
- Communication only via integration events

### Project Structure

```
src/
├── OrderTaking.Domain/           # Order domain models and value objects
├── OrderTaking.Infrastructure/   # EF Core DbContext, repositories, migrations
├── OrderTaking.Application/      # Application services, workflows
├── Billing.Domain/               # Invoice domain models
├── Billing.Infrastructure/       # EF Core DbContext, repositories, migrations
├── Billing.Application/          # Application services, workflows
├── Shipping.Domain/              # Shipment domain models
├── Shipping.Infrastructure/      # EF Core DbContext, repositories, migrations
├── Shipping.Application/         # Application services, workflows
├── Shared.Contracts/             # Integration events (OrderPlaced, InvoiceCreated, etc.)
├── Shared.Infrastructure/        # Messaging abstractions, Result<T>, utilities
└── Api/                          # Central ASP.NET Core API (controllers, startup)

tests/
├── OrderTaking.Tests/            # Unit & integration tests for OrderTaking
├── Billing.Tests/                # Unit & integration tests for Billing
└── Shipping.Tests/               # Unit & integration tests for Shipping
```

## Key Design Patterns

### 1. Domain-Driven Design (DDD)
- **Aggregates**: Order, Invoice, Shipment
- **Value Objects**: OrderId, Money, Address, OrderLineItem
- **Domain Events**: Emitted by aggregates to signal state changes
- **Repositories**: Abstract repository interfaces keep domain layer independent of EF Core

### 2. Typed Workflows
All workflows return `Result<TSuccess, TError>` instead of throwing exceptions:

```csharp
public record WorkflowResult(
    Success { Value: TSuccess },
    Failure { Error: TError }
);
```

This allows:
- Type-safe handling of success and failure paths
- No surprise exceptions for expected errors
- Clear error recovery logic
- Better control flow

### 3. Asynchronous Messaging
- **Message Bus**: MassTransit with RabbitMQ (dev: in-memory transport)
- **At-Least-Once Delivery**: Events guaranteed to reach subscribers
- **Idempotency**: Inbox pattern prevents duplicate processing
- **Reliability**: Outbox pattern ensures events are published before DB commit
- **DLQ**: Dead-letter queue for failed messages with retry policy

### 4. Tight Context Boundaries
```
OrderTaking -> publishes OrderPlaced event
   ↓
Billing (consumes OrderPlaced) -> creates Invoice -> publishes InvoiceCreated
Shipping (consumes OrderPlaced) -> creates Shipment -> publishes ShipmentCreated
   ↓
OrderTaking (optional: consumes for status tracking)
```

## Core Domain Types

### OrderTaking Bounded Context

**Aggregates:**
- `Order` - Root aggregate containing customer, line items, addresses, and status

**Value Objects:**
- `OrderId` - Strongly-typed order identifier
- `CustomerId` - Customer reference
- `Money` - Currency-aware amount
- `Address` - Shipping or billing address
- `OrderLineItem` - Product line in order
- `OrderStatus` - Enum: Pending, Confirmed, Cancelled, Completed

**Domain Invariants:**
- Order must have at least one line item
- Cannot add items to confirmed orders
- Cannot cancel completed orders
- Line item quantity must be positive

### Billing Bounded Context

**Aggregates:**
- `Invoice` - Root aggregate for billing

**Value Objects:**
- `InvoiceId` - Invoice identifier
- `OrderId` - Reference to OrderTaking context
- `Money` - Amount and currency
- `InvoiceStatus` - Enum: Created, Sent, Paid, Overdue, Cancelled

**Domain Invariants:**
- Cannot send invoice twice
- Cannot mark cancelled invoice as paid
- Amount cannot be negative

### Shipping Bounded Context

**Aggregates:**
- `Shipment` - Root aggregate for shipments

**Value Objects:**
- `ShipmentId` - Shipment identifier
- `OrderId` - Reference to OrderTaking context
- `Address` - Shipping address
- `ShipmentStatus` - Enum: Created, ReadyToShip, Dispatched, InTransit, Delivered, Failed, Cancelled

**Domain Invariants:**
- Cannot dispatch shipment without tracking number
- Cannot mark as delivered if not in transit
- Cannot cancel delivered shipment

## Integration Events

### OrderPlaced Event
Published by OrderTaking when order is confirmed.
Includes: OrderId, CustomerId, LineItems, TotalAmount, ShippingAddress

Consumers:
- Billing → Creates invoice
- Shipping → Creates shipment

### OrderCanceled Event
Published by OrderTaking when order is cancelled.

Consumers:
- Billing → Cancels related invoice
- Shipping → Cancels related shipment

### InvoiceCreated Event
Published by Billing when invoice is created from order.

### ShipmentCreated Event
Published by Shipping when shipment is created from order.

### ShipmentDispatched Event
Published by Shipping when shipment is dispatched with tracking number.

### ShipmentDelivered Event
Published by Shipping when shipment is delivered.

## Workflows (Typed Pipelines)

### Workflow: PlaceOrder (OrderTaking)
1. Validate order creation inputs (customer, line items, addresses)
2. Create Order aggregate with Pending status
3. Save to repository
4. Return `Success { Order }`
5. On publish, emit `OrderPlaced` integration event

### Workflow: ConfirmOrder (OrderTaking)
1. Retrieve order from repository
2. Validate order is in Pending state
3. Transition to Confirmed state
4. Save changes
5. Return `Success { Order }`
6. Emit `OrderPlaced` integration event

### Workflow: CancelOrder (OrderTaking)
1. Retrieve order from repository
2. Validate order can be cancelled
3. Transition to Cancelled state
4. Save changes
5. Return `Success { Order }`
6. Emit `OrderCanceled` integration event

### Workflow: CreateInvoiceFromOrder (Billing)
**Triggered by**: OrderPlaced event (async consumer)

1. Validate OrderPlaced event
2. Check if invoice already exists (idempotency: check Inbox)
3. Create Invoice aggregate with Created status
4. Save to repository
5. Mark message as processed in Inbox
6. Return `Success { Invoice }`
7. Emit `InvoiceCreated` integration event

### Workflow: CreateShipmentFromOrder (Shipping)
**Triggered by**: OrderPlaced event (async consumer)

1. Validate OrderPlaced event
2. Check if shipment already exists (idempotency)
3. Create Shipment aggregate with Created status
4. Calculate estimated delivery date (7 days from now)
5. Save to repository
6. Return `Success { Shipment }`
7. Emit `ShipmentCreated` integration event

## Messaging Architecture

### Outbox Pattern (Reliability)
When a workflow saves changes and publishes events:

```
BEGIN TRANSACTION
  1. Save aggregate to OrderDbContext
  2. Add event to Outbox table
COMMIT

Background worker:
  1. Poll Outbox for unpublished events
  2. Publish to message bus
  3. Mark as published in Outbox
```

This ensures events are only published if DB save succeeds.

### Inbox Pattern (Idempotency)
When consuming events:

```
BEGIN TRANSACTION
  1. Check Inbox - is this message already processed?
  2. If yes: return (idempotent, no duplicate processing)
  3. If no: process event, save domain changes
  4. Record message ID in Inbox
COMMIT
```

This prevents duplicate command execution if the same event is redelivered.

### Retry & DLQ Policy
```
First attempt: Immediate
Failed → Retry after 5 seconds
Failed → Retry after 30 seconds
Failed → Retry after 5 minutes
Failed → Move to DLQ (Dead Letter Queue)
```

DLQ messages can be inspected, manually recovered, or resubmitted.

## Database

### Connection String (LocalDB)
```
Server=(localdb)\mssqllocaldb;Database=OrderManagementSystem;Integrated Security=true;
```

### Per-Context Databases
- `OrderManagementSystem_OrderTaking` - Order aggregate data
- `OrderManagementSystem_Billing` - Invoice data
- `OrderManagementSystem_Shipping` - Shipment data
- `OrderManagementSystem_Shared` - Inbox/Outbox tables (shared infrastructure)

### Running Migrations
```bash
# OrderTaking migrations
cd src/OrderTaking.Infrastructure
dotnet ef migrations add InitialCreate --context OrderDbContext
dotnet ef database update --context OrderDbContext

# Billing migrations
cd src/Billing.Infrastructure
dotnet ef migrations add InitialCreate --context BillingDbContext
dotnet ef database update --context BillingDbContext

# Shipping migrations
cd src/Shipping.Infrastructure
dotnet ef migrations add InitialCreate --context ShippingDbContext
dotnet ef database update --context ShippingDbContext

# Shared infrastructure (Inbox/Outbox)
cd src/Shared.Infrastructure
dotnet ef migrations add InitialCreate --context SharedDbContext
dotnet ef database update --context SharedDbContext
```

## API Endpoints

### OrderTaking Endpoints

**POST /api/orders/place**
Request:
```json
{
  "customerId": "guid",
  "lineItems": [
    {
      "productId": "PROD001",
      "productName": "Widget",
      "quantity": 2,
      "unitPrice": { "amount": 29.99, "currencyCode": "USD" }
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "Springfield",
    "stateOrProvince": "IL",
    "postalCode": "62701",
    "countryCode": "US"
  },
  "billingAddress": { ... }
}
```

Response (Success):
```json
{
  "orderId": "guid",
  "status": "Pending",
  "totalAmount": { "amount": 59.98, "currencyCode": "USD" }
}
```

Response (Failure):
```json
{
  "error": "Order must have at least one line item"
}
```

**POST /api/orders/{orderId}/confirm**
Response:
```json
{
  "orderId": "guid",
  "status": "Confirmed"
}
```

**POST /api/orders/{orderId}/cancel**
Request:
```json
{
  "reason": "Customer requested cancellation"
}
```

**GET /api/orders/{orderId}**
Response:
```json
{
  "orderId": "guid",
  "customerId": "guid",
  "status": "Confirmed",
  "totalAmount": { "amount": 59.98, "currencyCode": "USD" },
  "lineItems": [ ... ],
  "createdAt": "2025-12-17T10:30:00Z"
}
```

**GET /api/orders?customerId={customerId}**
Response:
```json
[
  { "orderId": "guid", "status": "Confirmed", ... },
  { "orderId": "guid", "status": "Cancelled", ... }
]
```

### Billing Endpoints

**GET /api/invoices/{invoiceId}**
Response:
```json
{
  "invoiceId": "guid",
  "orderId": "guid",
  "amount": { "amount": 59.98, "currencyCode": "USD" },
  "status": "Sent",
  "sentAt": "2025-12-17T10:31:00Z"
}
```

**GET /api/invoices?orderId={orderId}**
Response:
```json
[
  { "invoiceId": "guid", "status": "Paid", "paidAt": "2025-12-18T14:00:00Z" }
]
```

**POST /api/invoices/{invoiceId}/mark-as-paid**
Response:
```json
{
  "invoiceId": "guid",
  "status": "Paid",
  "paidAt": "2025-12-18T14:00:00Z"
}
```

### Shipping Endpoints

**GET /api/shipments/{shipmentId}**
Response:
```json
{
  "shipmentId": "guid",
  "orderId": "guid",
  "status": "InTransit",
  "trackingNumber": "TRACK123456",
  "estimatedDeliveryDate": "2025-12-24T23:59:59Z",
  "dispatchedAt": "2025-12-19T08:00:00Z"
}
```

**GET /api/shipments?orderId={orderId}**
Response:
```json
[
  { "shipmentId": "guid", "status": "InTransit", "trackingNumber": "TRACK123456" }
]
```

**POST /api/shipments/{shipmentId}/dispatch**
Request:
```json
{
  "trackingNumber": "TRACK123456"
}
```

**POST /api/shipments/{shipmentId}/mark-as-delivered**
Response:
```json
{
  "shipmentId": "guid",
  "status": "Delivered",
  "deliveredAt": "2025-12-24T14:30:00Z"
}
```

## Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server LocalDB (or SQL Server)
- RabbitMQ (or use in-memory transport for dev)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/order-management-system.git
   cd order-management-system
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Create databases** (SQL Server LocalDB)
   ```bash
   # The solution will use LocalDB by default
   # Connection string: Server=(localdb)\mssqllocaldb;Database=OrderManagementSystem_*;Integrated Security=true;
   ```

5. **Run migrations**
   ```bash
   cd src/OrderTaking.Infrastructure
   dotnet ef database update
   
   cd ../Billing.Infrastructure
   dotnet ef database update
   
   cd ../Shipping.Infrastructure
   dotnet ef database update
   ```

6. **Start the API**
   ```bash
   cd src/Api
   dotnet run
   ```

   API will be available at: `https://localhost:7070`
   Swagger UI: `https://localhost:7070/swagger`

7. **Configure messaging** (optional for RabbitMQ)
   ```bash
   # Update appsettings.json with RabbitMQ connection
   # For development, in-memory transport is used by default
   ```

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests for specific context
dotnet test tests/OrderTaking.Tests
dotnet test tests/Billing.Tests
dotnet test tests/Shipping.Tests

# With coverage
dotnet test /p:CollectCoverage=true
```

## Git Workflow & Collaboration

This repository demonstrates a multi-developer workflow:

1. **Feature branches** for each bounded context
2. **Meaningful commit messages** showing incremental development
3. **Pull request reviews** between team members
4. **Squash commits** before merging to maintain clean history

Example workflow:
```bash
# Developer A: OrderTaking context
git checkout -b feature/order-workflows
# ... commits with clear messages
git push origin feature/order-workflows
# Create pull request

# Developer B: Billing context
git checkout -b feature/invoice-workflows
# ... commits
git push origin feature/invoice-workflows
# Create pull request

# Merge PRs independently
```

## Key Implementation Notes

### Error Handling
- Domain invariant violations throw `InvalidOperationException`
- Workflow validation errors return typed `Failure<TError>`
- HTTP layer converts domain errors to appropriate HTTP status codes

### Idempotency
- Each integration event has a unique `MessageId`
- Consumers check Inbox before processing
- If already processed, consumer returns success (idempotent)
- Prevents duplicate invoice/shipment creation if event is redelivered

### Correlation Tracing
- Each workflow creates a `CorrelationId`
- All events from same workflow share the same `CorrelationId`
- Enables end-to-end tracing: Order → Invoice → Shipment

### Performance
- Queries use filtered indexes on frequently searched columns
- Repository pattern enables optimized queries per context
- Outbox batch publishing to reduce DB load
- Message deduplication prevents redundant processing

### Deployment Considerations
- Each bounded context can be deployed independently
- Message bus is the only integration point
- Database migrations are context-specific
- Monitor DLQ for failed messages

## Future Enhancements

1. **Event Sourcing** - Store full event history instead of just current state
2. **CQRS** - Separate read models from write models
3. **Sagas** - Orchestrate complex multi-step processes (e.g., payment reconciliation)
4. **Temporal.io** - Replace custom workflows with Temporal for advanced orchestration
5. **Analytics** - Build analytics database from events
6. **Audit Trail** - Complete audit log of all state changes

## License

MIT

## Authors

This project demonstrates collaborative DDD development with contributions from multiple team members across different bounded contexts.

