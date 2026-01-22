# ================================================================
# Quick Setup Script - Order Management Database
# ================================================================
# This script automates the database setup process
# ================================================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Order Management Database Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$serverName = ".\SQLEXPRESS"  # Change this if you use LocalDB or different instance
$databaseName = "OrderManagement"
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "Configuration:" -ForegroundColor Yellow
Write-Host "  Server: $serverName" -ForegroundColor Gray
Write-Host "  Database: $databaseName" -ForegroundColor Gray
Write-Host "  Script Path: $scriptPath" -ForegroundColor Gray
Write-Host ""

# Step 1: Check if SQL Server is running
Write-Host "[Step 1/5] Checking SQL Server status..." -ForegroundColor Yellow
try {
    $sqlService = Get-Service -Name "MSSQL`$SQLEXPRESS" -ErrorAction Stop
    if ($sqlService.Status -eq "Running") {
        Write-Host "  ✓ SQL Server is running" -ForegroundColor Green
    } else {
        Write-Host "  ✗ SQL Server is not running" -ForegroundColor Red
        Write-Host "  Starting SQL Server..." -ForegroundColor Yellow
        Start-Service -Name "MSSQL`$SQLEXPRESS"
        Write-Host "  ✓ SQL Server started" -ForegroundColor Green
    }
} catch {
    Write-Host "  ⚠ Could not check SQL Server status" -ForegroundColor Yellow
    Write-Host "  Continuing anyway..." -ForegroundColor Gray
}
Write-Host ""

# Step 2: Test connection
Write-Host "[Step 2/5] Testing connection to SQL Server..." -ForegroundColor Yellow
try {
    $testQuery = "SELECT @@VERSION"
    $result = sqlcmd -S $serverName -E -Q $testQuery -h -1 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ Connection successful" -ForegroundColor Green
    } else {
        Write-Host "  ✗ Connection failed" -ForegroundColor Red
        Write-Host "  Error: $result" -ForegroundColor Red
        Write-Host ""
        Write-Host "Please check:" -ForegroundColor Yellow
        Write-Host "  1. SQL Server is installed" -ForegroundColor Gray
        Write-Host "  2. Instance name is correct (.\SQLEXPRESS or (localdb)\MSSQLLocalDB)" -ForegroundColor Gray
        Write-Host "  3. SQL Server service is running" -ForegroundColor Gray
        exit 1
    }
} catch {
    Write-Host "  ✗ sqlcmd not found" -ForegroundColor Red
    Write-Host "  Please install SQL Server Command Line Tools" -ForegroundColor Yellow
    Write-Host "  Or use SQL Server Management Studio to run the scripts manually" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

# Step 3: Create database and tables
Write-Host "[Step 3/5] Creating database and tables..." -ForegroundColor Yellow
$createDbScript = Join-Path $scriptPath "CreateOrderManagementDB.sql"
if (Test-Path $createDbScript) {
    Write-Host "  Running: CreateOrderManagementDB.sql" -ForegroundColor Gray
    $result = sqlcmd -S $serverName -E -i $createDbScript 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ Database and tables created successfully" -ForegroundColor Green
    } else {
        Write-Host "  ✗ Failed to create database" -ForegroundColor Red
        Write-Host "  Error: $result" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "  ✗ CreateOrderManagementDB.sql not found" -ForegroundColor Red
    Write-Host "  Expected location: $createDbScript" -ForegroundColor Gray
    exit 1
}
Write-Host ""

# Step 4: Seed sample data
Write-Host "[Step 4/5] Seeding sample data..." -ForegroundColor Yellow
$seedDataScript = Join-Path $scriptPath "SeedData.sql"
if (Test-Path $seedDataScript) {
    $response = Read-Host "  Do you want to insert sample data? (Y/N)"
    if ($response -eq "Y" -or $response -eq "y") {
        Write-Host "  Running: SeedData.sql" -ForegroundColor Gray
        $result = sqlcmd -S $serverName -E -i $seedDataScript 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ✓ Sample data inserted successfully" -ForegroundColor Green
        } else {
            Write-Host "  ✗ Failed to insert sample data" -ForegroundColor Red
            Write-Host "  Error: $result" -ForegroundColor Red
        }
    } else {
        Write-Host "  ⊘ Skipped sample data insertion" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ⚠ SeedData.sql not found, skipping..." -ForegroundColor Yellow
}
Write-Host ""

# Step 5: Verify setup
Write-Host "[Step 5/5] Verifying database setup..." -ForegroundColor Yellow
$verifyQuery = @"
USE OrderManagement;
SELECT 
    'Products' AS TableName, COUNT(*) AS RecordCount FROM Products
UNION ALL
SELECT 'Customers', COUNT(*) FROM Customers
UNION ALL
SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL
SELECT 'OrderItems', COUNT(*) FROM OrderItems;
"@

$result = sqlcmd -S $serverName -E -Q $verifyQuery -h -1 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ Database verification successful" -ForegroundColor Green
    Write-Host ""
    Write-Host "  Database contents:" -ForegroundColor Cyan
    Write-Host $result -ForegroundColor Gray
} else {
    Write-Host "  ✗ Verification failed" -ForegroundColor Red
}
Write-Host ""

# Display connection string
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Your connection string:" -ForegroundColor Yellow
Write-Host "Server=$serverName;Database=$databaseName;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Update your application's connection string" -ForegroundColor Gray
Write-Host "  2. Create the OrderManagement.Data project" -ForegroundColor Gray
Write-Host "  3. Add Entity Framework Core packages" -ForegroundColor Gray
Write-Host "  4. Create DTOs and DbContext" -ForegroundColor Gray
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

