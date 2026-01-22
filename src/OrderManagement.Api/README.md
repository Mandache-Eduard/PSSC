# 🚀 Order Management API

## Overview
RESTful Web API for managing orders in an online store, built with ASP.NET Core and DDD principles.

## Features
✅ **Swagger/OpenAPI Documentation** - Interactive API testing at root URL
✅ **Domain-Driven Design** - Clean architecture with separation of concerns
✅ **SQLite Database** - Lightweight, zero-configuration database
✅ **Async/Await** - Non-blocking I/O operations
✅ **Dependency Injection** - Built-in DI container
✅ **Validation** - Data annotation validation on API models

## Quick Start

### 1. Run the API
```powershell
cd C:\Users\manda\OneDrive\Desktop\PSSC\src\OrderManagement.Api
dotnet run
```

### 2. Access Swagger UI
Open your browser and navigate to:
```
https://localhost:5001
```
or
```
http://localhost:5000
```

The Swagger UI will appear, allowing you to test all API endpoints interactively.

## API Endpoints

### Orders

#### Get All Orders
```http
GET /api/orders
```

#### Get Order by Number
```http
GET /api/orders/{orderNumber}
```

#### Place New Order
```http
POST /api/orders
Content-Type: application/json

{
  "orderLines": [
    {
      "productCode": "AB1234",
      "quantity": 2
    }
  ],
  "street": "123 Main St",
  "city": "New York",
  "postalCode": "10001",
  "country": "USA"
}
```

#### Modify Order
```http
PUT /api/orders/{orderNumber}
Content-Type: application/json

[
  {
    "productCode": "CD5678",
    "quantity": 1
  }
]
```

#### Cancel Order
```http
DELETE /api/orders/{orderNumber}?reason=Customer%20request
```

#### Return Order
```http
POST /api/orders/{orderNumber}/return?returnReason=Defective
Content-Type: application/json

[
  {
    "productCode": "AB1234",
    "quantity": "1"
  }
]
```

## Configuration

### Connection String
Located in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../../../Data/OrderManagement.db"
  }
}
```

### Swagger Configuration
Located in `Program.cs`:
```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Management API",
        Version = "v1",
        Description = "REST API for managing orders"
    });
});
```

## Project Structure
```
OrderManagement.Api/
├── Controllers/
│   └── OrdersController.cs      # API endpoints
├── Models/
│   ├── InputOrderLine.cs        # DTOs
│   └── PlaceOrderRequest.cs
├── Program.cs                   # API configuration & DI
├── appsettings.json            # Configuration
└── OrderManagement.Api.csproj  # Project file
```

## Dependencies
- OrderManagement.Domain (Business logic)
- OrderManagement.Data (Data access)
- Microsoft.EntityFrameworkCore.Sqlite
- Swashbuckle.AspNetCore (Swagger)

## Architecture

### Request Flow
```
HTTP Request
    ↓
Controller (API Layer)
    ↓
Map to Domain Command
    ↓
Workflow (Domain Layer)
    ↓
Repository (Data Layer)
    ↓
SQLite Database
    ↓
Domain Event
    ↓
Map to HTTP Response
    ↓
HTTP Response
```

### Separation of Concerns
- **API Models** (DTOs) - Mutable, validation attributes
- **Domain Models** - Immutable, value objects
- **Mapping** - Controller maps between API and Domain

## Testing with Swagger

1. **Run the API** - `dotnet run`
2. **Open Swagger UI** - Navigate to https://localhost:5001
3. **Expand an endpoint** - Click on any endpoint
4. **Try it out** - Click "Try it out" button
5. **Fill parameters** - Enter request data
6. **Execute** - Click "Execute" button
7. **View response** - See the response below

## Sample Workflow Test

### 1. Place an Order
```json
POST /api/orders
{
  "orderLines": [
    { "productCode": "AB1234", "quantity": 1 },
    { "productCode": "CD5678", "quantity": 2 }
  ],
  "street": "123 Main St",
  "city": "New York",
  "postalCode": "10001",
  "country": "USA"
}
```

**Expected Response:**
```json
{
  "message": "Order placed successfully",
  "orderNumber": "ORD-1234567890",
  "totalAmount": 1059.97,
  "orderDate": "2026-01-22T10:30:00"
}
```

### 2. Get the Order
```http
GET /api/orders/ORD-1234567890
```

### 3. Modify the Order (within 48 hours)
```http
PUT /api/orders/ORD-1234567890
[
  { "productCode": "EF9012", "quantity": 1 }
]
```

### 4. Cancel the Order (within 24 hours)
```http
DELETE /api/orders/ORD-1234567890?reason=Changed my mind
```

## Environment-Specific Settings

### Development
Uses `appsettings.Development.json` (if exists)
- Swagger UI enabled
- Detailed error messages
- Debug logging

### Production
Uses `appsettings.json`
- Swagger UI disabled (optional)
- Secure error messages
- Info/Warning logging

## Troubleshooting

### Port Already in Use
Change ports in `Properties/launchSettings.json`:
```json
{
  "applicationUrl": "https://localhost:7001;http://localhost:5001"
}
```

### Database Not Found
Ensure the database exists at:
```
C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db
```

Run the Console app once to create it:
```powershell
cd ..\OrderManagement.Console
dotnet run
```

### Swagger Not Appearing
1. Verify you're in Development mode
2. Check the URL is correct (root path)
3. Clear browser cache

## Next Steps

- [ ] Add authentication/authorization
- [ ] Implement pagination for GetAllOrders
- [ ] Add response caching
- [ ] Implement rate limiting
- [ ] Add correlation IDs for request tracking
- [ ] Create integration tests
- [ ] Add health check endpoints

## Resources

- [Swagger UI](https://swagger.io/tools/swagger-ui/)
- [ASP.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

