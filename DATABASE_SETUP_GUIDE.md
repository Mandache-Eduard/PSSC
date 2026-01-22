# 🗄️ DATABASE SETUP GUIDE - Order Management System

## Overview
This guide will help you set up a **SQLite database** for your Order Management System. SQLite is perfect for local development - it requires **ZERO installation** and works on any platform!

---

## 📋 **Why SQLite?**

### **Perfect for Your Needs** ✅
- ✅ **No installation required** - Just a file!
- ✅ **No server configuration** - Works immediately
- ✅ **Cross-platform** - Windows, Mac, Linux
- ✅ **Lightweight** - Database is just one file
- ✅ **Fast** - Great for development
- ✅ **Easy to backup** - Just copy the file
- ✅ **Perfect for learning** - Simple and straightforward

### **SQLite vs SQL Server**

| Feature | SQLite | SQL Server |
|---------|--------|------------|
| Installation | None needed ✅ | 15+ minutes ❌ |
| Configuration | Zero ✅ | Complex ❌ |
| File size | One .db file ✅ | Multiple files ❌ |
| Services | None ✅ | Must run service ❌ |
| Portability | 100% ✅ | Limited ❌ |
| Learning curve | Easy ✅ | Steeper ❌ |

### **SQLite is Used By:**
- Mobile apps (Android, iOS)
- Web browsers (Chrome, Firefox)
- Desktop applications
- Embedded systems
- And thousands of other applications!

---

## 🚀 **SETUP STEPS** (Super Easy!)

### **Step 1: No Installation Needed!** ✅

That's right - **you don't need to install anything**! SQLite works through Entity Framework Core packages in your .NET project.

**What you need:**
- ✅ Visual Studio or VS Code (you already have this)
- ✅ .NET SDK (you already have this)
- ✅ That's it!

---

### **Step 2: Create the Database File Location**

The database will be a single file. Let's create a folder for it:

```powershell
# Create a Data folder in your project
New-Item -Path "C:\Users\manda\OneDrive\Desktop\PSSC\Data" -ItemType Directory -Force
```

Your database file will be: `C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db`

**That's it for setup!** No services, no configuration, no complexity. ✅

---

## 🗃️ **STEP 3: Install Entity Framework Core for SQLite**

We need to add SQLite support to your projects:

### **For OrderManagement.Data Project** (we'll create this next)

```powershell
cd C:\Users\manda\OneDrive\Desktop\PSSC\src\OrderManagement.Data
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### **For OrderManagement.Console Project**

```powershell
cd C:\Users\manda\OneDrive\Desktop\PSSC\src\OrderManagement.Console
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

**That's all the "installation" you need!** 🎉

---

## 🔗 **STEP 4: Your Connection String**

Your connection string is **super simple**:

```
Data Source=C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db
```

**Or use a relative path:**

```
Data Source=../../../Data/OrderManagement.db
```

**Explanation:**
- `Data Source=` - Points to the database file
- `OrderManagement.db` - Your database file (created automatically!)

**No server names, no authentication, no complexity!** ✅

---

## 🏗️ **STEP 5: Create the Database Schema**

I've created a C# script that will create your database automatically using Entity Framework Core migrations.

**The database will be created automatically when you run your application for the first time!**

But if you want to create it manually, I'll provide a script below.

---

## ✅ **STEP 6: Verify Database Creation**

After running your application once, check if the database file exists:

```powershell
Test-Path "C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db"
```

If it returns `True`, your database exists! ✅

### **View Your Database**

You can use several tools to view your SQLite database:

#### **Option 1: DB Browser for SQLite** (Free, GUI) ⭐ RECOMMENDED
- Download: https://sqlitebrowser.org/dl/
- Open your `.db` file
- View tables, run queries, edit data

#### **Option 2: SQLite CLI** (Command line)
- Download: https://www.sqlite.org/download.html
- Run: `sqlite3 OrderManagement.db`
- Type: `.tables` to see all tables

#### **Option 3: VS Code Extension**
- Install: "SQLite" extension by alexcvzz
- Right-click `.db` file → **Open Database**
- View tables in VS Code

#### **Option 4: Visual Studio**
- Install: "SQLite/SQL Server Compact Toolbox" extension
- Right-click `.db` file → **Add Connection**

---

## 🧪 **STEP 7: Seed Initial Data**

I've created a C# seeder class that will populate your database with sample data.

Run this command to seed data:

```powershell
cd C:\Users\manda\OneDrive\Desktop\PSSC\src\OrderManagement.Console
dotnet run -- seed
```

This will add:
- 10 sample products (Laptop, Mouse, Keyboard, etc.)
- 5 sample customers
- Ready for testing!

---

## 🛠️ **Troubleshooting**

### **Problem 1: "SQLite Error 1: 'table already exists'"**

**Solution:** Database tables already created. Either:
- Delete the database file and recreate it
- Or continue using existing database

```powershell
# Delete and recreate
Remove-Item "C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db"
# Then run your application again
```

---

### **Problem 2: "Unable to open the database file"**

**Solution 1:** Check file path is correct
```powershell
# Verify path exists
Test-Path "C:\Users\manda\OneDrive\Desktop\PSSC\Data"
# Create if missing
New-Item -Path "C:\Users\manda\OneDrive\Desktop\PSSC\Data" -ItemType Directory -Force
```

**Solution 2:** Check file permissions
- Make sure you have write access to the Data folder
- Try running as administrator (right-click PowerShell → Run as Administrator)

---

### **Problem 3: "Package not found" when adding SQLite**

**Solution:** Make sure you're in the right directory and have internet connection
```powershell
# Check you're in project folder
cd C:\Users\manda\OneDrive\Desktop\PSSC\src\OrderManagement.Data
# Try again
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

---

### **Problem 4: "Database is locked"**

**Solution:** Another program has the database open
- Close DB Browser for SQLite (if open)
- Close any other SQLite viewers
- Stop your application
- Try again

---

### **Problem 5: Can't see database file**

**Solution:** Database hasn't been created yet
```powershell
# Check if file exists
Get-ChildItem "C:\Users\manda\OneDrive\Desktop\PSSC\Data\*.db"
# If not, run your application first to create it
```

---

## 📊 **Database Schema Overview**

```
┌─────────────────────────────────────┐
│          Products                    │
├─────────────────────────────────────┤
│ ProductId (PK, INT, Identity)       │
│ Code (VARCHAR(10), Unique)          │
│ Name (VARCHAR(100))                 │
│ Price (DECIMAL(18,2))               │
│ Stock (INT)                         │
│ IsActive (BIT)                      │
└─────────────────────────────────────┘
            ↑
            │
┌───────────┴─────────────────────────┐
│         OrderItems                   │
├─────────────────────────────────────┤
│ OrderItemId (PK, INT, Identity)     │
│ OrderId (FK → Orders)               │
│ ProductId (FK → Products)           │
│ Quantity (DECIMAL(18,2))            │
│ Price (DECIMAL(18,2))               │
│ LineTotal (DECIMAL(18,2))           │
└─────────────────────────────────────┘
            │
            ↓
┌─────────────────────────────────────┐
│            Orders                    │
├─────────────────────────────────────┤
│ OrderId (PK, INT, Identity)         │
│ OrderNumber (VARCHAR(50), Unique)   │
│ CustomerId (FK → Customers)         │
│ OrderDate (DATETIME)                │
│ TotalAmount (DECIMAL(18,2))         │
│ Status (VARCHAR(20))                │
│ Street (VARCHAR(200))               │
│ City (VARCHAR(100))                 │
│ PostalCode (VARCHAR(20))            │
│ Country (VARCHAR(100))              │
└─────────────────────────────────────┘
            │
            ↓
┌─────────────────────────────────────┐
│          Customers                   │
├─────────────────────────────────────┤
│ CustomerId (PK, INT, Identity)      │
│ Code (VARCHAR(20), Unique)          │
│ Name (VARCHAR(100))                 │
│ Email (VARCHAR(100))                │
│ IsActive (BIT)                      │
└─────────────────────────────────────┘
```

---

## 🎯 **Next Steps**

After your database is set up:

1. ✅ **Choose Database** - SQLite (Done!)
2. ✅ **Install Packages** - Entity Framework Core SQLite
3. 📦 **Create Data Layer Project** - Next step
4. 🔧 **Create DTOs** - Next step
5. 📝 **Create DbContext** - Next step
6. 🏗️ **Create Repositories** - Next step
7. ⚡ **Update Workflows** - Next step

---

## 📞 **Getting Help**

### **Check if database file exists:**
```powershell
Test-Path "C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db"
```

### **View database file info:**
```powershell
Get-Item "C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db" | Select-Object Name, Length, LastWriteTime
```

### **Delete and recreate (if needed):**
```powershell
Remove-Item "C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db"
# Then run your application to recreate
```

---

## ✅ **Quick Checklist**

Before proceeding, make sure:

- [ ] .NET SDK is installed (you already have this)
- [ ] Data folder created: `C:\Users\manda\OneDrive\Desktop\PSSC\Data`
- [ ] SQLite NuGet packages will be added (next step)
- [ ] Connection string ready: `Data Source=../../../Data/OrderManagement.db`
- [ ] (Optional) DB Browser for SQLite downloaded for viewing database

---

## 💡 **Advantages of SQLite for Your Project**

### **For Learning:**
- ✅ Focus on code, not database administration
- ✅ No service management complexity
- ✅ Quick setup and teardown
- ✅ Perfect for experimenting

### **For Development:**
- ✅ Fast iterations
- ✅ Easy to reset (just delete the file!)
- ✅ Version control friendly (can commit the .db file if small)
- ✅ Works offline

### **For Migration:**
- ✅ When ready for production, switching to SQL Server/PostgreSQL is easy
- ✅ Entity Framework Core makes it database-agnostic
- ✅ Just change connection string and provider
- ✅ Your domain code stays the same!

---

**Once you complete this setup, we'll move on to creating the Data Layer project!** 🚀


