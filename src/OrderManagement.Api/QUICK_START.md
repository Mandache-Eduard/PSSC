# 🎯 QUICK START - Order Management API with Swagger

## ⚡ Run the API (3 seconds)
```powershell
cd C:\Users\manda\OneDrive\Desktop\PSSC\src\OrderManagement.Api
dotnet run
```

## 🌐 Open Swagger UI
```
https://localhost:5001
```

## ✅ What You'll See
- **Interactive API documentation**
- **Try it out** buttons to test endpoints
- **Request/Response examples**
- **Validation rules displayed**

---

## 📍 Available Endpoints

| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/api/orders` | Get all orders |
| GET | `/api/orders/{orderNumber}` | Get specific order |
| POST | `/api/orders` | Place new order |
| PUT | `/api/orders/{orderNumber}` | Modify order |
| DELETE | `/api/orders/{orderNumber}?reason=...` | Cancel order |
| POST | `/api/orders/{orderNumber}/return?returnReason=...` | Return order |

---

## 🧪 Quick Test

### 1. Place Order (in Swagger UI)
Click **POST /api/orders** → **Try it out** → Paste:
```json
{
  "orderLines": [
    {"productCode": "AB1234", "quantity": 1},
    {"productCode": "CD5678", "quantity": 2}
  ],
  "street": "123 Main St",
  "city": "New York",
  "postalCode": "10001",
  "country": "USA"
}
```
Click **Execute**

### 2. Get Order
Copy `orderNumber` from response → **GET /api/orders/{orderNumber}** → **Try it out** → Paste order number → **Execute**

---

## 📦 Project Structure
```
OrderManagement.Api/
├── Controllers/OrdersController.cs  ← 6 REST endpoints
├── Models/                          ← API DTOs
├── Program.cs                       ← Swagger config
└── appsettings.json                 ← Connection string
```

---

## 🔧 Swagger Configuration
Located in `Program.cs`:
- ✅ Serves at root URL (https://localhost:5001/)
- ✅ Development environment only
- ✅ OpenAPI v3 specification
- ✅ Auto-generated from controllers

---

## 💡 Key Features
- ✅ **Zero configuration** needed - just run!
- ✅ **Interactive testing** - no Postman needed
- ✅ **Auto-documentation** - from code attributes
- ✅ **Validation** - automatic error responses
- ✅ **DDD architecture** - domain logic isolated

---

## 🎨 Swagger UI Features
- **Expand/Collapse** endpoints
- **Try it out** - interactive testing
- **Response examples** - see what to expect
- **Models** - view request/response schemas
- **Download spec** - OpenAPI JSON/YAML

---

## 🚨 Troubleshooting

### Port already in use?
Edit `Properties/launchSettings.json` → change ports

### Swagger not loading?
1. Check you're running in Development mode
2. Navigate to root URL (not /swagger)
3. Clear browser cache

### Database errors?
Run Console app once to create database:
```powershell
cd ..\OrderManagement.Console
dotnet run
```

---

## 📚 Resources
- **API README:** `OrderManagement.Api/README.md`
- **Setup Guide:** `SWAGGER_SETUP_COMPLETE.md`
- **Analysis:** `L6_L7_ANALYSIS_WEB_API_DDD.md`

---

**That's it! Your API is ready to use.** 🚀

Just run `dotnet run` and open your browser!

