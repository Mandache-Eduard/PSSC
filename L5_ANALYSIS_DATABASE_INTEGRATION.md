# 📚 L5 ANALYSIS - Database Integration in DDD Architecture

## Summary
Analysis of **PSSC\Examples\L5** - Identifying new elements for database read/write operations in a Domain-Driven Design architecture.

---

## 🆕 **NEW ELEMENTS IDENTIFIED**

### 1. **Separate Data Layer Project** ⭐ NEW!

**Project Structure:**
```
Solution/
├── Exemple (Console Application)
├── Exemple.Domain (Domain Layer - Pure Business Logic)
└── Example.Data (Data Layer - Database Access) ← NEW!
```

**Example.Data Project** contains:
- `GradesContext.cs` - Entity Framework DbContext
- `Models/` - Database DTOs (Data Transfer Objects)
- `Repositories/` - Repository implementations

**Why Separate:**
- ✅ **Separation of Concerns** - Domain logic isolated from database
- ✅ **Testability** - Can test domain without database
- ✅ **Flexibility** - Can swap data layer without touching domain
- ✅ **Clean Architecture** - Domain doesn't depend on infrastructure

---

### 2. **DTOs (Data Transfer Objects)** ⭐ NEW!

**Purpose:** Separate database models from domain models

#### StudentDto.cs
```csharp
public class StudentDto
{
    public int StudentId { get; set; }               // Database ID (mutable)
    public string Name { get; set; } = string.Empty; // Mutable
    public string RegistrationNumber { get; set; } = string.Empty;
}
```

#### GradeDto.cs
```csharp
public class GradeDto
{
    public int GradeId { get; set; }      // Database ID
    public int StudentId { get; set; }    // Foreign key
    public decimal? Exam { get; set; }    // Nullable for database
    public decimal? Activity { get; set; }
    public decimal? Final { get; set; }
}
```

**Key Characteristics:**
- ❌ **Mutable** - Have setters (required by Entity Framework)
- ❌ **No validation** - Plain data containers
- ❌ **Database-focused** - Have IDs, foreign keys, nullable types
- ✅ **Anemic** - No behavior, just data

**vs Domain Models:**
- ✅ **Immutable** - Records with get-only properties
- ✅ **Validated** - TryParse patterns, business rules
- ✅ **Rich** - Contain behavior and business logic
- ✅ **No database concerns** - No IDs, no foreign keys

---

### 3. **Entity Framework Core DbContext** ⭐ NEW!

#### GradesContext.cs
```csharp
public class GradesContext : DbContext
{
    public GradesContext(DbContextOptions<GradesContext> options) : base(options)
    {
    }

    // DbSets - represent database tables
    public DbSet<GradeDto> Grades { get; set; }
    public DbSet<StudentDto> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure table mappings
        modelBuilder
            .Entity<StudentDto>()
            .ToTable("Student")           // Table name
            .HasKey(s => s.StudentId);    // Primary key

        modelBuilder
            .Entity<GradeDto>(entityBuilder =>
            {
                entityBuilder
                    .Property(g => g.Activity)
                    .HasColumnType("decimal(18, 0)");  // Column type mapping

                entityBuilder
                    .ToTable("Grade")
                    .HasKey(s => s.GradeId);
            });
    }
}
```

**What it does:**
- ✅ Manages database connection
- ✅ Tracks entity changes
- ✅ Provides LINQ query interface
- ✅ Maps objects to database tables

---

### 4. **Repository Pattern** ⭐ NEW!

**Two-layer approach:**

#### Layer 1: Interface in Domain (Abstractions)

**IGradesRepository.cs** (in Exemple.Domain)
```csharp
public interface IGradesRepository
{
    // Returns DOMAIN models, not DTOs!
    Task<List<CalculatedStudentGrade>> GetExistingGradesAsync();
    
    // Accepts DOMAIN models, not DTOs!
    Task SaveGradesAsync(PublishedExam grades);
}
```

**IStudentsRepository.cs** (in Exemple.Domain)
```csharp
public interface IStudentsRepository
{
    // Works with DOMAIN types (StudentRegistrationNumber)
    Task<List<StudentRegistrationNumber>> GetExistingStudentsAsync(IEnumerable<string> studentsToCheck);
}
```

**Key Points:**
- ✅ Defined in **Domain layer**
- ✅ Uses **domain types** (not DTOs)
- ✅ Domain doesn't know about database
- ✅ **Async** - All methods return Task<>

---

#### Layer 2: Implementation in Data Layer

**GradesRepository.cs** (in Example.Data)
```csharp
public class GradesRepository : IGradesRepository
{
    private readonly GradesContext dbContext;

    public GradesRepository(GradesContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<List<CalculatedStudentGrade>> GetExistingGradesAsync()
    {
        // 1. Load DTOs from database
        var foundStudentGrades = await (
            from g in dbContext.Grades
            join s in dbContext.Students on g.StudentId equals s.StudentId
            select new { s.RegistrationNumber, g.GradeId, g.Exam, g.Activity, g.Final }
        ).AsNoTracking()
         .ToListAsync();

        // 2. Map DTOs to Domain Models
        List<CalculatedStudentGrade> foundGradesModel = foundStudentGrades.Select(result =>
            new CalculatedStudentGrade(
                StudentRegistrationNumber: new StudentRegistrationNumber(result.RegistrationNumber),
                ExamGrade: result.Exam is null ? null : new Grade(result.Exam.Value),
                ActivityGrade: result.Activity is null ? null : new Grade(result.Activity.Value),
                FinalGrade: result.Final is null ? null : new Grade(result.Final.Value))
            {
                GradeId = result.GradeId
            })
         .ToList();

        return foundGradesModel;
    }

    public async Task SaveGradesAsync(PublishedExam exam)
    {
        // Load students lookup
        ILookup<string, StudentDto> students = (await dbContext.Students.ToListAsync())
            .ToLookup(student => student.RegistrationNumber);
        
        // Add new grades
        AddNewGrades(exam, students);
        
        // Update existing grades
        UpdateExistingGrades(exam, students);
        
        // Save changes to database
        await dbContext.SaveChangesAsync();
    }
}
```

**Repository Responsibilities:**
1. ✅ **Load data** from database (DTOs)
2. ✅ **Map DTOs → Domain Models** (for reading)
3. ✅ **Map Domain Models → DTOs** (for writing)
4. ✅ **Execute database operations** (CRUD)
5. ✅ **Hide database details** from domain

---

### 5. **Async/Await Pattern** ⭐ NEW!

All database operations are **asynchronous**:

```csharp
// Methods return Task<T>
public async Task<List<CalculatedStudentGrade>> GetExistingGradesAsync()
{
    // Use await for database calls
    var data = await dbContext.Grades.ToListAsync();
    return data;
}

// Workflow is async
public async Task<IExamPublishedEvent> ExecuteAsync(PublishExamCommand command)
{
    // Await repository calls
    List<StudentRegistrationNumber> existingStudents = 
        await studentsRepository.GetExistingStudentsAsync(studentsToCheck);
    
    // Await save operations
    await gradesRepository.SaveGradesAsync(publishedExam);
    
    return exam.ToEvent();
}
```

**Why async:**
- ✅ **Non-blocking I/O** - Don't block thread during database calls
- ✅ **Scalability** - Better server resource utilization
- ✅ **Responsive UI** - Keeps application responsive
- ✅ **Modern .NET** - Standard practice for I/O operations

---

### 6. **Workflow Pattern with Database** ⭐ NEW!

**PublishExamWorkflow.cs** - 3-Step Pattern:

```csharp
public async Task<IExamPublishedEvent> ExecuteAsync(PublishExamCommand command)
{
    try
    {
        // STEP 1: LOAD STATE FROM DATABASE
        IEnumerable<string> studentsToCheck = 
            command.InputExamGrades.Select(grade => grade.StudentRegistrationNumber);
        List<StudentRegistrationNumber> existingStudents = 
            await studentsRepository.GetExistingStudentsAsync(studentsToCheck);
        List<CalculatedStudentGrade> existingGrades = 
            await gradesRepository.GetExistingGradesAsync();

        // STEP 2: EXECUTE PURE BUSINESS LOGIC (no database!)
        IExam exam = ExecuteBusinessLogic(command, existingStudents, existingGrades);

        // STEP 3: SAVE NEW STATE TO DATABASE
        if (exam is PublishedExam publishedExam)
        {
            await gradesRepository.SaveGradesAsync(publishedExam);
        }

        // Generate event
        return exam.ToEvent();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while publishing grades");
        return new ExamPublishFailedEvent("Unexpected error");
    }
}

private static IExam ExecuteBusinessLogic(
    PublishExamCommand command,
    List<StudentRegistrationNumber> existingStudents,
    List<CalculatedStudentGrade> existingGrades)
{
    // Pure functions - no database access!
    Func<StudentRegistrationNumber, bool> checkStudentExists = 
        student => existingStudents.Any(s => s.Equals(student));
    
    UnvalidatedExam unvalidatedGrades = new(command.InputExamGrades);

    IExam exam = new ValidateExamOperation(checkStudentExists).Transform(unvalidatedGrades);
    exam = new CalculateExamOperation().Transform(exam, existingGrades);
    exam = new PublishExamOperation().Transform(exam);
    return exam;
}
```

**3-Step Pattern:**
```
┌─────────────────────────────────────────────────────────┐
│ 1. LOAD STATE (async database read)                    │
│    - Read existing data from database                  │
│    - Convert DTOs → Domain Models                      │
└─────────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│ 2. EXECUTE BUSINESS LOGIC (pure, synchronous)          │
│    - No database access!                               │
│    - Pure domain operations                            │
│    - Uses loaded state for validation                  │
└─────────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│ 3. SAVE STATE (async database write)                   │
│    - Convert Domain Models → DTOs                      │
│    - Save to database                                  │
└─────────────────────────────────────────────────────────┘
```

**Why separate business logic:**
- ✅ **Testable** - Can test without database
- ✅ **Pure** - No side effects
- ✅ **Fast** - No I/O during logic execution
- ✅ **Reusable** - Same logic works with different storage

---

### 7. **Dependency Injection in Main** ⭐ NEW!

**Program.cs** - Manual DI setup:

```csharp
private static async Task Main(string[] args)
{
    // 1. Configure logging
    using ILoggerFactory loggerFactory = ConfigureLoggerFactory();
    ILogger<PublishExamWorkflow> logger = loggerFactory.CreateLogger<PublishExamWorkflow>();
    
    // 2. Configure DbContext with connection string
    DbContextOptionsBuilder<GradesContext> dbContextBuilder = 
        new DbContextOptionsBuilder<GradesContext>()
            .UseSqlServer(ConnectionString)
            .UseLoggerFactory(loggerFactory);
    
    // 3. Create DbContext
    GradesContext gradesContext = new(dbContextBuilder.Options);
    
    // 4. Create repositories (inject DbContext)
    StudentsRepository studentsRepository = new(gradesContext);
    GradesRepository gradesRepository = new(gradesContext);

    // 5. Get user input
    UnvalidatedStudentGrade[] listOfGrades = ReadListOfGrades().ToArray();

    // 6. Execute workflow (inject repositories)
    PublishExamCommand command = new(listOfGrades);
    PublishExamWorkflow workflow = new(studentsRepository, gradesRepository, logger);
    IExamPublishedEvent result = await workflow.ExecuteAsync(command);

    // 7. Display result
    string consoleMessage = result switch
    {
        ExamPublishSucceededEvent @event => @event.Csv,
        ExamPublishFailedEvent @event => $"Publish failed: {string.Join("\\r\\n", @event.Reasons)}",
        _ => throw new NotImplementedException()
    };

    Console.WriteLine(consoleMessage);
}
```

**Dependency Chain:**
```
DbContext 
    ↓ injected into
Repositories
    ↓ injected into
Workflow
    ↓ uses
Domain Operations (pure logic)
```

---

### 8. **EF Core Query Patterns** ⭐ NEW!

#### AsNoTracking() - Read-only queries
```csharp
var foundStudentGrades = await (
    from g in dbContext.Grades
    join s in dbContext.Students on g.StudentId equals s.StudentId
    select new { s.RegistrationNumber, g.GradeId, g.Exam, g.Activity, g.Final }
).AsNoTracking()  // ← Don't track changes (faster for read-only)
 .ToListAsync();
```

**Why AsNoTracking:**
- ✅ **Performance** - No change tracking overhead
- ✅ **Memory** - Entities aren't kept in change tracker
- ✅ **Read-only** - Clearly signals intent

---

#### Entity State Management - Updates
```csharp
private void UpdateExistingGrades(PublishedExam exam, ILookup<string, StudentDto> students)
{
    IEnumerable<GradeDto> updatedGrades = exam.GradeList
        .Where(g => g.IsUpdated && g.GradeId > 0)
        .Select(g => new GradeDto()
        {
            GradeId = g.GradeId,
            StudentId = students[g.StudentRegistrationNumber.Value].Single().StudentId,
            Exam = g.ExamGrade?.Value,
            Activity = g.ActivityGrade?.Value,
            Final = g.FinalGrade?.Value,
        });

    foreach (GradeDto entity in updatedGrades)
    {
        dbContext.Entry(entity).State = EntityState.Modified;  // ← Mark as modified
    }
}
```

---

#### Adding New Entities
```csharp
private void AddNewGrades(PublishedExam exam, ILookup<string, StudentDto> students)
{
    IEnumerable<GradeDto> newGrades = exam.GradeList
        .Where(g => !g.IsUpdated && g.GradeId == 0)
        .Select(g => new GradeDto()
        {
            StudentId = students[g.StudentRegistrationNumber.Value].Single().StudentId,
            Exam = g.ExamGrade?.Value,
            Activity = g.ActivityGrade?.Value,
            Final = g.FinalGrade?.Value,
        });
    
    dbContext.AddRange(newGrades);  // ← Add to context (INSERT)
}
```

---

#### SaveChanges - Persistence
```csharp
await dbContext.SaveChangesAsync();  // ← Executes all pending changes
```

**What SaveChanges does:**
- ✅ Generates SQL (INSERT, UPDATE, DELETE)
- ✅ Executes in transaction
- ✅ Updates entity IDs (for new entities)
- ✅ Clears change tracker

---

### 9. **Logging Integration** ⭐ NEW!

```csharp
private readonly ILogger<PublishExamWorkflow> logger;

public PublishExamWorkflow(
    IStudentsRepository studentsRepository, 
    IGradesRepository gradesRepository, 
    ILogger<PublishExamWorkflow> logger)  // ← Inject logger
{
    this.logger = logger;
}

public async Task<IExamPublishedEvent> ExecuteAsync(PublishExamCommand command)
{
    try
    {
        // ... workflow logic ...
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while publishing grades");
        return new ExamPublishFailedEvent("Unexpected error");
    }
}
```

**Logging Configuration:**
```csharp
private static ILoggerFactory ConfigureLoggerFactory()
{
    return LoggerFactory.Create(builder =>
        builder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "hh:mm:ss ";
        })
        .AddProvider(new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()));
}
```

---

### 10. **Database Schema** ⭐ NEW!

**create-db.sql** - SQL Server database:

```sql
CREATE TABLE [dbo].[Student](
    [StudentId] [int] IDENTITY(1,1) NOT NULL,  -- Auto-increment PK
    [RegistrationNumber] [varchar](7) NOT NULL,
    [Name] [varchar](50) NOT NULL,
    CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED ([StudentId] ASC)
)

CREATE TABLE [dbo].[Grade](
    [GradeId] [int] IDENTITY(1,1) NOT NULL,    -- Auto-increment PK
    [StudentId] [int] NOT NULL,                 -- Foreign key
    [Exam] [decimal](18, 2) NULL,               -- Nullable
    [Activity] [decimal](18, 2) NULL,
    [Final] [decimal](18, 2) NULL,
    CONSTRAINT [PK_Grades] PRIMARY KEY CLUSTERED ([GradeId] ASC)
)

ALTER TABLE [dbo].[Grade] 
    ADD CONSTRAINT [FK_Grades_Student] FOREIGN KEY([StudentId])
    REFERENCES [dbo].[Student] ([StudentId])
```

---

## 📊 **Architecture Comparison**

### Without Database (L3)
```
User Input
    ↓
Command
    ↓
Workflow (pure composition)
    ↓
Operations (pure functions)
    ↓
Event
    ↓
Output
```

### With Database (L5)
```
User Input
    ↓
Command
    ↓
Workflow (async, orchestrates I/O)
    ↓ ← Load State (Repository → DbContext → Database)
    ↓
Operations (pure functions - no database!)
    ↓
    ↓ → Save State (Repository → DbContext → Database)
Event
    ↓
Output
```

---

## 🎯 **Key Patterns Summary**

| Pattern | Purpose | Location |
|---------|---------|----------|
| **DTO** | Database entity | Data layer |
| **Domain Model** | Business logic | Domain layer |
| **Repository Interface** | Abstraction | Domain layer |
| **Repository Implementation** | Data access | Data layer |
| **DbContext** | EF Core database connection | Data layer |
| **Async/Await** | Non-blocking I/O | Throughout |
| **3-Step Workflow** | Load → Logic → Save | Workflow |
| **Dependency Injection** | Loose coupling | Main/Startup |

---

## ✅ **Best Practices Demonstrated**

### 1. **Separation of Concerns** ✅
- Domain doesn't know about database
- Data layer doesn't contain business logic

### 2. **Dependency Inversion** ✅
- Domain defines interfaces (IGradesRepository)
- Data layer implements them (GradesRepository)
- Domain depends on abstractions, not concretions

### 3. **Pure Business Logic** ✅
- Operations have no database access
- Testable without database
- Fast execution (no I/O)

### 4. **Explicit Mapping** ✅
- DTOs ↔ Domain Models clearly separated
- Mapping happens in repository
- Domain stays clean

### 5. **Async All The Way** ✅
- All I/O operations are async
- Better scalability
- Modern .NET practice

---

## 🆚 **DTO vs Domain Model**

| Aspect | DTO (StudentDto) | Domain Model (StudentRegistrationNumber) |
|--------|------------------|------------------------------------------|
| **Purpose** | Database mapping | Business logic |
| **Mutability** | Mutable (setters) | Immutable (get only) |
| **Validation** | None | TryParse, business rules |
| **Behavior** | Anemic (data only) | Rich (behavior) |
| **Database IDs** | Has StudentId | No database concerns |
| **Nullability** | Allows nulls | Typed (non-null) |
| **Layer** | Data layer | Domain layer |

---

## 📚 **New Concepts to Apply**

For your Order Management System, you'll need:

### 1. **Create Data Layer Project**
- `OrderManagement.Data.csproj`
- Add Entity Framework Core packages

### 2. **Define DTOs**
- `ProductDto` - for Products table
- `CustomerDto` - for Customers table
- `OrderDto` - for Order table
- `OrderItemDto` - for OrderItem table

### 3. **Create DbContext**
- `OrderManagementContext`
- Configure table mappings
- Define relationships

### 4. **Define Repository Interfaces** (in Domain)
- `IProductsRepository`
- `ICustomersRepository`
- `IOrdersRepository`

### 5. **Implement Repositories** (in Data)
- Map DTOs ↔ Domain Models
- Implement CRUD operations
- Use AsNoTracking for queries

### 6. **Update Workflows**
- Make them async
- Load state from database
- Execute pure logic
- Save results to database

### 7. **Setup Dependency Injection**
- Configure DbContext
- Register repositories
- Inject into workflows

---

## 🎓 **Learning Summary**

**New in L5:**
- ✅ **Data Layer** - Separate project for database concerns
- ✅ **DTOs** - Mutable database entities
- ✅ **Entity Framework Core** - ORM for database access
- ✅ **Repository Pattern** - Abstraction over data access
- ✅ **Async/Await** - Non-blocking I/O operations
- ✅ **3-Step Workflow** - Load → Logic → Save pattern
- ✅ **DTO↔Domain Mapping** - Explicit conversions
- ✅ **Dependency Injection** - Manual DI setup
- ✅ **Logging** - ILogger integration
- ✅ **SQL Database** - Relational database with relationships

**Architecture stays clean:**
- ✅ Domain layer is still pure
- ✅ Operations don't know about database
- ✅ Workflows orchestrate but don't contain business logic
- ✅ Type safety maintained
- ✅ Immutability preserved in domain

**Ready to implement in your Order Management System!** 🚀


