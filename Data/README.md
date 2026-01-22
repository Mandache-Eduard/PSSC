# Data Folder - SQLite Database

## 📁 What's This Folder?

This folder contains your SQLite database file for the Order Management System.

---

## 📄 Files Here

- **OrderManagement.db** - Your SQLite database file (created automatically)
- **OrderManagement.db-shm** - Shared memory file (temporary, created during use)
- **OrderManagement.db-wal** - Write-Ahead Log file (temporary, for performance)

**Only `OrderManagement.db` is permanent!**

---

## ✅ Database Creation

The database file will be created **automatically** when you:
1. Run your application for the first time
2. Entity Framework Core creates the schema
3. Migrations are applied

**You don't need to create it manually!** 🎉

---

## 🔗 Connection String

From your application, use this relative path:

```
Data Source=../../../Data/OrderManagement.db
```

Or absolute path:
```
Data Source=C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db
```

---

## 🗄️ What's Inside the Database

After creation, your database will contain:

### **Tables:**
- **Products** - Product catalog (10 sample items)
- **Customers** - Customer information (5 sample customers)
- **Orders** - Order headers
- **OrderItems** - Order line items

### **Relationships:**
- Orders → Customers (many-to-one)
- OrderItems → Orders (many-to-one)
- OrderItems → Products (many-to-one)

---

## 🔍 View Your Database

### **Option 1: DB Browser for SQLite** ⭐ RECOMMENDED
1. Download: https://sqlitebrowser.org/dl/
2. Open `OrderManagement.db`
3. Browse tables, run queries, edit data

### **Option 2: VS Code Extension**
1. Install "SQLite" extension by alexcvzz
2. Right-click `OrderManagement.db` → Open Database
3. View tables in VS Code

### **Option 3: Online Viewer**
1. Go to: https://sqliteviewer.app/
2. Upload `OrderManagement.db`
3. View in browser (no installation)

---

## 🧹 Reset Database

To start fresh:

```powershell
# Delete the database file
Remove-Item "C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db"

# Then run your application - it will recreate
```

---

## 💾 Backup Your Database

SQLite makes backup **super easy** - just copy the file!

```powershell
# Backup
Copy-Item "OrderManagement.db" "OrderManagement.backup.db"

# Restore
Copy-Item "OrderManagement.backup.db" "OrderManagement.db" -Force
```

**Pro tip:** Stop your application before copying!

---

## 📊 Database Size

A typical SQLite database for this project:
- **Empty:** ~20 KB
- **With sample data:** ~50-100 KB
- **With 1000 orders:** ~500 KB - 1 MB

**SQLite is incredibly compact!** ✅

---

## 🚫 .gitignore

Add this to your `.gitignore` to avoid committing database files:

```
# SQLite database files
*.db
*.db-shm
*.db-wal
```

**Exception:** You might want to commit an initial database with seed data for other developers.

---

## ⚙️ Performance Tips

### **For Better Performance:**
1. **Use transactions** for bulk inserts
2. **Create indexes** on frequently queried columns
3. **Use Write-Ahead Logging** (WAL mode - default in EF Core)
4. **Regular VACUUM** to optimize file size

### **WAL Mode** (Automatic with EF Core)
```sql
PRAGMA journal_mode = WAL;
```
✅ Better concurrency  
✅ Faster writes  
✅ Safer transactions  

---

## 🔒 Security

### **File Permissions:**
- Only your user account should have access
- Windows file permissions apply
- No password needed for local development

### **For Production:**
Consider:
- Encrypt the database file (SQLCipher)
- Set restrictive file permissions
- Or migrate to SQL Server/PostgreSQL

---

## 🐛 Troubleshooting

### **Problem: "Database is locked"**
**Solution:**
- Close DB Browser for SQLite
- Close other programs accessing the file
- Stop your application

### **Problem: "Cannot open database"**
**Solution:**
```powershell
# Check file exists
Test-Path "OrderManagement.db"

# Check file is readable
Get-Acl "OrderManagement.db"
```

### **Problem: "Corrupted database"**
**Solution:**
```powershell
# Try to recover
sqlite3 OrderManagement.db ".recover" > recovery.sql

# Or restore from backup
Copy-Item "OrderManagement.backup.db" "OrderManagement.db" -Force
```

---

## 📚 Learn More

- SQLite Documentation: https://www.sqlite.org/docs.html
- EF Core with SQLite: https://learn.microsoft.com/ef/core/providers/sqlite/
- DB Browser for SQLite: https://sqlitebrowser.org/

---

**Your database is ready to use!** 🎉

Just run your application and Entity Framework Core will create everything automatically!

