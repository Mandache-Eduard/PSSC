# 📊 L6 & L7 Analysis - Web API with DDD Implementation

## 🎯 Overview

This document identifies the **new elements** and architectural patterns introduced in L6 (Web API with DDD) and L7 (Synchronous Communication between APIs) examples.

---

## 📁 L6: Web API Implementation with DDD Model

### **New Project Structure**

```
Example.Api/           ← NEW: Web API Project
├── Controllers/       ← NEW: API Controllers
├── Models/           ← NEW: API Input/Output models (DTOs)
├── Program.cs        ← NEW: API Configuration & Dependency Injection
├── appsettings.json  ← NEW: Configuration files
└── Properties/

Example.Data/         ← Existing: Data Layer
Example.Domain/       ← Existing: Domain Layer
```

---

## 🆕 **NEW ELEMENTS in L6**

### **1. Web API Project (Example.Api)**

**Key Characteristics:**
- ASP.NET Core Web API
- RESTful endpoints
- JSON serialization
- HTTP request/response handling

---

### **2. API Controllers**

**File:** `Controllers/GradesController.cs`

**New Concepts:**

#### **a) Controller Attributes**
```csharp
[ApiController]              // Enables API-specific behaviors
[Route("[controller]")]      // Routing pattern: /Grades
```

#### **b) HTTP Verb Attributes**
```csharp
[HttpGet("getAllGrades")]    // GET request mapping
[HttpPost]                   // POST request mapping
```

#### **c) Action Results**
```csharp
public async Task<IActionResult> PublishGrades(...)
{
    return Ok();              // HTTP 200
    return BadRequest(...);   // HTTP 400
}
```

#### **d) Dependency Injection in Actions**
```csharp
public async Task<IActionResult> GetAllGrades(
    [FromServices] IGradesRepository gradesRepository)  // NEW: Service injection in action
```

#### **e) Request Body Binding**
```csharp
public async Task<IActionResult> PublishGrades(
    [FromBody] InputGrade[] grades)  // NEW: JSON body deserialization
```

---

### **3. API Models (DTOs)**

**File:** `Models/InputGrade.cs`

**Purpose:** Separate API contract from Domain models

**New Concepts:**

#### **a) Data Annotations**
```csharp
public class InputGrade
{
    [Required]                                          // Validation: field required
    [RegularExpression(StudentRegistrationNumber.Pattern)] // Pattern validation
    public string RegistrationNumber { get; set; }
    
    [Range(1, 10)]                                      // Range validation
    public decimal Exam { get; set; }
}
```

**Key Differences from Domain Models:**
- ✅ **Mutable** (setters allowed)
- ✅ **Validation attributes** for automatic validation
- ✅ **Simple types** (strings, decimals) - not value objects
- ✅ **API contract** - decoupled from domain

---

### **4. Program.cs - API Configuration**

**New Concepts:**

#### **a) WebApplicationBuilder Pattern**
```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
```

#### **b) Service Registration (Dependency Injection)**
```csharp
// Database context
builder.Services.AddDbContext<GradesContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository registration
builder.Services.AddTransient<IGradesRepository, GradesRepository>();
builder.Services.AddTransient<IStudentsRepository, StudentsRepository>();

// Workflow registration
builder.Services.AddTransient<PublishExamWorkflow>();

// HTTP Client factory
builder.Services.AddHttpClient();
```

**Service Lifetimes:**
- **AddTransient**: New instance per request
- **AddScoped**: One instance per HTTP request
- **AddSingleton**: One instance for application lifetime

#### **c) Swagger/OpenAPI Configuration**
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Example.Api", Version = "v1" });
});
```

**Purpose:** Auto-generate API documentation and testing UI

#### **d) Middleware Pipeline**
```csharp
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();        // Swagger JSON endpoint
    app.UseSwaggerUI();      // Swagger UI
}

app.UseHttpsRedirection();   // Redirect HTTP to HTTPS
app.UseAuthorization();      // Authorization middleware
app.MapControllers();        // Map controller routes
app.Run();                   // Start the application
```

---

### **5. Workflow Integration with API**

**Pattern:** Controller → Workflow → Event → Response

```csharp
// 1. Map API input to Domain command
ReadOnlyCollection<UnvalidatedStudentGrade> unvalidatedGrades = grades
    .Select(MapInputGradeToUnvalidatedGrade)
    .ToList()
    .AsReadOnly();

// 2. Create command
PublishExamCommand command = new(unvalidatedGrades);

// 3. Execute workflow
IExamPublishedEvent workflowResult = await publishGradeWorkflow.ExecuteAsync(command);

// 4. Match event to HTTP response
IActionResult response = workflowResult switch
{
    ExamPublishSucceededEvent @event => Ok(),
    ExamPublishFailedEvent @event => BadRequest(@event.Reasons),
    _ => throw new NotImplementedException()
};
```

**Key Pattern:** 
- API layer maps to/from domain
- Domain remains pure and isolated
- Events drive HTTP responses

---

### **6. Configuration Files**

#### **appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

**Purpose:** External configuration for connection strings, logging, etc.

---

## 📁 L7: Synchronous API Communication

### **New Project Structure**

```
Example.Api/              ← Enhanced with HTTP clients
├── Clients/             ← NEW: Typed HTTP clients
│   └── ReportApiClient.cs
├── Filters/             ← NEW: Swagger filters
│   └── ExcludeControllersDocumentFilter.cs

Example.ReportGenerator/ ← NEW: Separate API Service
├── Controllers/
│   └── ReportController.cs
├── Models/
│   └── ExamPublishedModel.cs
└── Program.cs
```

---

## 🆕 **NEW ELEMENTS in L7**

### **1. Typed HTTP Client**

**File:** `Clients/ReportApiClient.cs`

**New Concepts:**

#### **a) Primary Constructor Pattern (C# 12)**
```csharp
public class ReportApiClient(HttpClient httpClient)  // NEW: Primary constructor
{
    // httpClient is automatically available as a field
}
```

#### **b) Typed Client Methods**
```csharp
public async Task<string> GenerateReportAsync(ExamPublishedModel examPublished)
{
    StringContent content = new(
        JsonSerializer.Serialize(examPublished), 
        Encoding.UTF8, 
        "application/json");
    
    HttpResponseMessage response = await httpClient.PostAsync(
        "report/semester-report", 
        content);
    
    response.EnsureSuccessStatusCode();  // Throws on non-success
    return await response.Content.ReadAsStringAsync();
}
```

**Benefits:**
- ✅ Encapsulates HTTP communication logic
- ✅ Strongly typed
- ✅ Testable (can mock HttpClient)
- ✅ Reusable across application

---

### **2. HTTP Client Registration with Polly**

**File:** `Program.cs`

#### **a) Typed Client Registration**
```csharp
builder.Services.AddHttpClient<ReportApiClient>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7286");
    })
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));
```

**New Concepts:**

**AddHttpClient<T>:** Registers typed client with DI
- Automatically manages HttpClient lifecycle
- Prevents socket exhaustion
- Enables configuration and policies

**ConfigureHttpClient:** Sets base address and default headers

**AddPolicyHandler:** Adds Polly resilience policies

---

### **3. Polly Resilience Policies**

**Purpose:** Handle transient failures (network issues, timeouts, etc.)

#### **a) Transient Error Handling**
```csharp
HttpPolicyExtensions
    .HandleTransientHttpError()  // Handles 5xx and network errors
    .WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600))
```

**What it does:**
- Automatically retries failed requests
- Waits 600ms between retries
- Retries up to 3 times
- Handles HTTP 5xx and network exceptions

**Retry Pattern:**
```
Request → Fail → Wait 600ms → Retry (1/3)
       → Fail → Wait 600ms → Retry (2/3)
       → Fail → Wait 600ms → Retry (3/3)
       → Fail → Return error
```

**Alternative Patterns:**
```csharp
// Exponential backoff
.WaitAndRetryAsync(3, retryAttempt => 
    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))

// Circuit breaker
.CircuitBreakerAsync(3, TimeSpan.FromMinutes(1))
```

---

### **4. Parallel API Calls**

**File:** `Controllers/GradesController.cs`

```csharp
private async Task<IActionResult> PublishEvent(ExamPublishSucceededEvent successEvent)
{
    ExamPublishedModel dto = new()
    {
        Csv = successEvent.Csv,
        PublishedDate = successEvent.PublishedDate
    };

    // NEW: Execute multiple API calls in parallel
    Task w1 = _reportApiClient.GenerateReportAsync(dto);
    Task w2 = _reportApiClient.CalculateScholarshipAsync(dto);
    await Task.WhenAll(w1, w2);  // Wait for both to complete
    
    return Ok();
}
```

**Benefits:**
- ✅ Faster execution (parallel vs sequential)
- ✅ Better resource utilization
- ✅ All-or-nothing completion semantics

---

### **5. Swagger Document Filters**

**File:** `Filters/ExcludeControllersDocumentFilter.cs`

**Purpose:** Customize Swagger documentation

```csharp
public class ExcludeControllersDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var controllerToExclude = "Report";
        
        // Remove specific endpoints from Swagger UI
        var keys = swaggerDoc.Paths.Keys
            .Where(path => path.Contains(controllerToExclude))
            .ToList();
            
        foreach (var key in keys)
        {
            swaggerDoc.Paths.Remove(key);
        }
        
        // Remove schemas
        swaggerDoc.Components.Schemas.Remove("ExamPublishedModel");
    }
}
```

**Registration:**
```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Example.Api", Version = "v1" });
    c.DocumentFilter<ExcludeControllersDocumentFilter>();  // Apply filter
});
```

**Use Case:** Hide internal APIs from public documentation

---

### **6. Separate API Service (Microservice)**

**Project:** `Example.ReportGenerator`

**New Pattern:** Multiple independent APIs

#### **Report Controller**
```csharp
[ApiController]
[Route("[controller]")]
public class ReportController : ControllerBase
{
    [HttpPost("semester-report")]
    public IActionResult GenerateReport([FromBody] ExamPublishedModel examPublished)
    {
        _logger.LogInformation($"Landed on GenerateReport Action {examPublished.Csv}");
        return Ok("Report generated successfully");
    }
    
    [HttpPost("scholarship")]
    public IActionResult ScholarshipCalculation([FromBody] ExamPublishedModel examPublished)
    {
        _logger.LogInformation($"Landed on ScholarshipCalculation Action {examPublished.Csv}");
        return Ok("Scholarship calculated successfully");
    }
}
```

**Architecture Pattern:**
```
Example.Api (Main API)
    ↓ HTTP POST
Example.ReportGenerator (Service API)
    └── /report/semester-report
    └── /report/scholarship
```

---

## 🏗️ **Architecture Comparison**

### **Console Application (L1-L5)**
```
User Input → Workflow → Database → Console Output
```

### **Web API (L6)**
```
HTTP Request → Controller → Workflow → Database → HTTP Response
```

### **Microservices (L7)**
```
HTTP Request → Controller → Workflow → Database
                    ↓
              HTTP Client (with Polly)
                    ↓
              External API → Process → HTTP Response
```

---

## 📊 **Key Architectural Patterns**

### **1. Separation of Concerns**

| Layer | L1-L5 | L6 | L7 |
|-------|-------|----|----|
| **Presentation** | Console | API Controllers | API Controllers |
| **API Models** | - | InputGrade (DTOs) | ExamPublishedModel |
| **Application** | Program.cs | Program.cs + Startup | Program.cs + HTTP Clients |
| **Domain** | Workflows, Operations | Workflows, Operations | Workflows, Operations |
| **Data** | Repositories, DbContext | Repositories, DbContext | Repositories, DbContext |
| **Infrastructure** | - | - | HTTP Clients, Polly |

---

### **2. Dependency Injection Evolution**

**L1-L5 (Manual):**
```csharp
var context = new OrderManagementContext(options);
var repository = new OrdersRepository(context);
var workflow = new PlaceOrderWorkflow(repository, ...);
```

**L6-L7 (DI Container):**
```csharp
// Registration (once)
builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();
builder.Services.AddTransient<PlaceOrderWorkflow>();

// Usage (automatic injection)
public class OrdersController
{
    public OrdersController(PlaceOrderWorkflow workflow) { }
}
```

**Benefits:**
- ✅ Centralized configuration
- ✅ Automatic lifetime management
- ✅ Easier testing
- ✅ Looser coupling

---

### **3. API Contract Pattern**

**Separation:**
```
API Layer (DTOs)     ←→     Domain Layer (Value Objects)
    ↓                            ↓
InputGrade                   UnvalidatedStudentGrade
(mutable, simple)            (immutable, validated)
```

**Mapping:**
```csharp
private static UnvalidatedStudentGrade MapInputGradeToUnvalidatedGrade(InputGrade grade) 
    => new(
        StudentRegistrationNumber: grade.RegistrationNumber,
        ExamGrade: grade.Exam,
        ActivityGrade: grade.Activity
    );
```

---

## ✅ **Summary of New Elements**

### **L6 - Web API Fundamentals:**
1. ✅ **API Controllers** with routing and HTTP verbs
2. ✅ **API Models (DTOs)** with validation attributes
3. ✅ **Dependency Injection** container and service registration
4. ✅ **Swagger/OpenAPI** documentation
5. ✅ **Configuration** files (appsettings.json)
6. ✅ **Middleware pipeline** configuration
7. ✅ **Action Results** (Ok, BadRequest, etc.)
8. ✅ **Model binding** ([FromBody], [FromServices])
9. ✅ **Service lifetimes** (Transient, Scoped, Singleton)

### **L7 - API Communication:**
1. ✅ **Typed HTTP Clients** for service-to-service communication
2. ✅ **Polly** resilience policies (retry, circuit breaker)
3. ✅ **Primary constructors** (C# 12 feature)
4. ✅ **Parallel async operations** (Task.WhenAll)
5. ✅ **Swagger filters** for documentation customization
6. ✅ **Microservices architecture** (multiple APIs)
7. ✅ **HTTP Client factory** pattern
8. ✅ **Exponential backoff** and retry strategies
9. ✅ **Service-to-service DTOs** (ExamPublishedModel)

---

## 🎯 **Key Takeaways**

### **Architecture Evolution:**
```
L1-L5: Console → Workflows → Database
L6:    HTTP → API → Workflows → Database → HTTP
L7:    HTTP → API → Workflows → Database → External APIs → HTTP
```

### **Design Principles Applied:**
- ✅ **Clean Architecture** (API ← Domain → Data)
- ✅ **Dependency Inversion** (interfaces in domain, implementations in data/api)
- ✅ **Single Responsibility** (controllers orchestrate, workflows execute logic)
- ✅ **Open/Closed** (extend via DI, not modification)
- ✅ **Separation of Concerns** (API models ≠ Domain models)

### **Modern .NET Features:**
- ✅ Minimal APIs setup (WebApplicationBuilder)
- ✅ Primary constructors
- ✅ Record types for DTOs
- ✅ Pattern matching in controllers
- ✅ Top-level statements

---

**This analysis covers all major new elements introduced in L6 and L7 for implementing Web APIs with DDD architecture.** 🚀

