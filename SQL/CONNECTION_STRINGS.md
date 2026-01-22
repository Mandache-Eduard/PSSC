# SQLite Connection String Reference

## Your Connection String

SQLite is **super simple** - just point to a file!

---

## **Basic Connection String** (Absolute Path)

```
Data Source=C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db
```

---

## **Recommended** (Relative Path)

```
Data Source=../../../Data/OrderManagement.db
```

**Why relative?** 
- ✅ Works from any location in your project
- ✅ Portable across different machines
- ✅ Works in version control

---

## **Advanced Options**

### **With Mode (Read/Write)**
```
Data Source=C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db;Mode=ReadWriteCreate
```

**Modes:**
- `ReadWriteCreate` - Creates file if doesn't exist (default)
- `ReadWrite` - Requires file to exist
- `ReadOnly` - Read-only access

### **With Cache**
```
Data Source=C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db;Cache=Shared
```

### **In-Memory Database** (For Testing)
```
Data Source=:memory:
```
**Warning:** Data is lost when connection closes!

---

## **Connection String Parts Explained**

| Part | Meaning |
|------|---------|
| `Data Source=` | Path to your .db file |
| `OrderManagement.db` | Database filename |
| `Mode=ReadWriteCreate` | (Optional) Access mode |
| `Cache=Shared` | (Optional) Cache mode |

---

## **How to Use in Your Code**

### **In appsettings.json** (future use):
```json
{
  "ConnectionStrings": {
    "OrderManagementDb": "Data Source=../../../Data/OrderManagement.db"
  }
}
```

### **In Program.cs** (current use):
```csharp
private static readonly string ConnectionString = 
    "Data Source=../../../Data/OrderManagement.db";
```

### **In DbContext configuration:**
```csharp
DbContextOptionsBuilder<OrderManagementContext> builder = 
    new DbContextOptionsBuilder<OrderManagementContext>()
        .UseSqlite(ConnectionString);
```

---

## **Testing Your Connection String**

### **Using PowerShell:**
```powershell
# Check if database file exists
Test-Path "C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db"

# View file info
Get-Item "C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db"
```

### **Using C# (quick test):**
```csharp
using Microsoft.Data.Sqlite;

string connectionString = "Data Source=../../../Data/OrderManagement.db";

try
{
    using var connection = new SqliteConnection(connectionString);
    connection.Open();
    Console.WriteLine("✓ Connection successful!");
    Console.WriteLine($"Database: {connection.DataSource}");
}
catch (Exception ex)
{
    Console.WriteLine("✗ Connection failed!");
    Console.WriteLine(ex.Message);
}
```

---

## **Common Connection String Patterns**

### **Development (Local File):**
```
Data Source=../../../Data/OrderManagement.db
```
✅ Perfect for local development

### **Testing (In-Memory):**
```
Data Source=:memory:
```
✅ Fast, isolated tests

### **Production (Absolute Path):**
```
Data Source=/var/lib/myapp/OrderManagement.db
```
✅ Server deployment

### **Temp Database:**
```
Data Source=C:\Temp\OrderManagement.db;Mode=ReadWriteCreate
```
✅ Temporary testing

---

## **Common Issues**

### **Issue 1: "Unable to open database"**
**Solution:** Check path exists
```powershell
# Create Data folder if missing
New-Item -Path "C:\Users\manda\OneDrive\Desktop\PSSC\Data" -ItemType Directory -Force
```

### **Issue 2: "Database is locked"**
**Solution:** Another program has file open
- Close DB Browser for SQLite
- Close VS Code SQLite extension
- Stop your application

### **Issue 3: "Cannot find database file"**
**Solution:** Use correct relative path
```csharp
// From Console project:
Data Source=../../../Data/OrderManagement.db

// From Data project:
Data Source=../../../../Data/OrderManagement.db
```

### **Issue 4: Relative path not working**
**Solution:** Use absolute path temporarily
```csharp
// Absolute (for debugging)
Data Source=C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db
```

---

## **Security Notes**

### **For Development (Current):**
✅ Local file - No password needed  
✅ File permissions control access  
✅ Perfect for local development  

### **For Production (Future):**
⚠️ Set proper file permissions  
⚠️ Consider encryption (SQLCipher)  
⚠️ Backup regularly (just copy the file!)  
⚠️ Or migrate to SQL Server/PostgreSQL  

---

## **Quick Reference Table**

| Use Case | Connection String |
|----------|-------------------|
| Development | `Data Source=../../../Data/OrderManagement.db` |
| Testing | `Data Source=:memory:` |
| Absolute Path | `Data Source=C:\...\Data\OrderManagement.db` |
| Read-Only | `Data Source=...\OrderManagement.db;Mode=ReadOnly` |

---

## **File Location Tips**

### **Recommended Structure:**
```
PSSC/
├── Data/
│   └── OrderManagement.db  ← Database file here
├── src/
│   ├── OrderManagement.Console/
│   │   └── Program.cs  ← Use: ../../../Data/OrderManagement.db
│   └── OrderManagement.Data/
│       └── OrderManagementContext.cs
```

### **Why this works:**
- ✅ Database outside source code folder
- ✅ Can be .gitignored easily
- ✅ Easy to backup separately
- ✅ Clean separation

---

## **Migration from SQLite to SQL Server** (Future)

When ready for production:

**SQLite:**
```csharp
.UseSqlite("Data Source=OrderManagement.db")
```

**SQL Server:**
```csharp
.UseSqlServer("Server=.;Database=OrderManagement;...")
```

**PostgreSQL:**
```csharp
.UseNpgsql("Host=localhost;Database=OrderManagement;...")
```

**Your domain code stays the same!** ✅

---

**Copy the connection string and use it in your DbContext configuration!**

```
Data Source=../../../Data/OrderManagement.db
```

---

## **SQL Server Express (Most Common)**

### **Default Instance:**
```
Server=.\SQLEXPRESS;Database=OrderManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true
```

### **Alternative Formats:**
```
Server=localhost\SQLEXPRESS;Database=OrderManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true
```

```
Server=(local)\SQLEXPRESS;Database=OrderManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true
```

---

## **SQL Server LocalDB**

```
Server=(localdb)\MSSQLLocalDB;Database=OrderManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true
```

---

## **SQL Server Developer/Enterprise**

```
Server=.;Database=OrderManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true
```

---

## **Connection String Parts Explained**

| Part | Meaning |
|------|---------|
| `Server=.\SQLEXPRESS` | SQL Server instance name |
| `Database=OrderManagement` | Database name |
| `Trusted_Connection=True` | Use Windows Authentication |
| `MultipleActiveResultSets=true` | Allow multiple queries simultaneously |
| `TrustServerCertificate=true` | Trust the server certificate (for local dev) |

---

## **How to Use in Your Code**

### **In appsettings.json (future use):**
```json
{
  "ConnectionStrings": {
    "OrderManagementDb": "Server=.\\SQLEXPRESS;Database=OrderManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### **In Program.cs (current use):**
```csharp
private static readonly string ConnectionString = 
    "Server=.\\SQLEXPRESS;Database=OrderManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true";
```

### **In DbContext configuration:**
```csharp
DbContextOptionsBuilder<OrderManagementContext> builder = 
    new DbContextOptionsBuilder<OrderManagementContext>()
        .UseSqlServer(ConnectionString);
```

---

## **Testing Your Connection String**

### **Using PowerShell:**
```powershell
# Replace YOUR_CONNECTION_STRING with your actual connection string
$connectionString = "Server=.\SQLEXPRESS;Database=OrderManagement;Trusted_Connection=True"
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
try {
    $connection.Open()
    Write-Host "✓ Connection successful!" -ForegroundColor Green
    Write-Host "Database: $($connection.Database)" -ForegroundColor Cyan
    $connection.Close()
} catch {
    Write-Host "✗ Connection failed!" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}
```

### **Using C# (quick test):**
```csharp
using System.Data.SqlClient;

string connectionString = "Server=.\\SQLEXPRESS;Database=OrderManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true";

try
{
    using SqlConnection connection = new(connectionString);
    connection.Open();
    Console.WriteLine("✓ Connection successful!");
    Console.WriteLine($"Database: {connection.Database}");
}
catch (Exception ex)
{
    Console.WriteLine("✗ Connection failed!");
    Console.WriteLine(ex.Message);
}
```

---

## **Common Connection String Issues**

### **Issue 1: "Cannot open database"**
**Solution:** Database doesn't exist. Run `CreateOrderManagementDB.sql`

### **Issue 2: "Login failed"**
**Solution:** Use `Trusted_Connection=True` for Windows Authentication

### **Issue 3: "Server not found"**
**Solution:** Check server name. Try:
- `.\SQLEXPRESS`
- `localhost\SQLEXPRESS`
- `(localdb)\MSSQLLocalDB`

### **Issue 4: "Certificate trust issue"**
**Solution:** Add `TrustServerCertificate=true` to connection string

---

## **Security Notes**

### **For Development (Current):**
✅ `Trusted_Connection=True` is perfect
✅ `TrustServerCertificate=true` is acceptable
✅ No password in connection string
✅ Uses your Windows account

### **For Production (Future):**
⚠️ Use SQL Server Authentication with strong password
⚠️ Store connection string in secure configuration
⚠️ Use proper SSL certificate
⚠️ Consider Azure SQL Database or managed instances

---

## **Quick Reference Table**

| Setup Type | Connection String |
|------------|-------------------|
| SQL Server Express | `Server=.\SQLEXPRESS;Database=OrderManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true` |
| SQL Server LocalDB | `Server=(localdb)\MSSQLLocalDB;Database=OrderManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true` |
| SQL Server Default | `Server=.;Database=OrderManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true` |

---

**Copy the connection string that matches your setup and use it in your application!**


