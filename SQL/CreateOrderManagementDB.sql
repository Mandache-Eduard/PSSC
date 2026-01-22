-- ================================================================
-- Order Management System Database Creation Script
-- ================================================================
-- This script creates the database and all required tables
-- for the Order Management System
-- ================================================================

USE master;
GO

-- Drop database if it exists (for clean setup)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'OrderManagement')
BEGIN
    ALTER DATABASE OrderManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE OrderManagement;
    PRINT 'Existing OrderManagement database dropped.';
END
GO

-- Create the database
CREATE DATABASE OrderManagement;
GO

PRINT 'OrderManagement database created successfully.';
GO

-- Switch to the new database
USE OrderManagement;
GO

-- ================================================================
-- Table 1: Products
-- ================================================================
-- Stores product catalog information
-- ================================================================

CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) NOT NULL,
    Code VARCHAR(10) NOT NULL,
    Name VARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (ProductId ASC),
    CONSTRAINT UQ_Products_Code UNIQUE (Code),
    CONSTRAINT CK_Products_Price CHECK (Price >= 0),
    CONSTRAINT CK_Products_Stock CHECK (Stock >= 0)
);
GO

PRINT 'Products table created successfully.';
GO

-- ================================================================
-- Table 2: Customers
-- ================================================================
-- Stores customer information
-- ================================================================

CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) NOT NULL,
    Code VARCHAR(20) NOT NULL,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Customers PRIMARY KEY CLUSTERED (CustomerId ASC),
    CONSTRAINT UQ_Customers_Code UNIQUE (Code),
    CONSTRAINT UQ_Customers_Email UNIQUE (Email)
);
GO

PRINT 'Customers table created successfully.';
GO

-- ================================================================
-- Table 3: Orders
-- ================================================================
-- Stores order header information
-- ================================================================

CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) NOT NULL,
    OrderNumber VARCHAR(50) NOT NULL,
    CustomerId INT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Status VARCHAR(20) NOT NULL DEFAULT 'Confirmed',
    Street VARCHAR(200) NULL,
    City VARCHAR(100) NULL,
    PostalCode VARCHAR(20) NULL,
    Country VARCHAR(100) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedDate DATETIME NULL,
    CONSTRAINT PK_Orders PRIMARY KEY CLUSTERED (OrderId ASC),
    CONSTRAINT UQ_Orders_OrderNumber UNIQUE (OrderNumber),
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) 
        REFERENCES Customers(CustomerId),
    CONSTRAINT CK_Orders_TotalAmount CHECK (TotalAmount >= 0),
    CONSTRAINT CK_Orders_Status CHECK (Status IN ('Confirmed', 'Cancelled', 'Returned', 'Shipped', 'Delivered'))
);
GO

PRINT 'Orders table created successfully.';
GO

-- ================================================================
-- Table 4: OrderItems
-- ================================================================
-- Stores individual items within an order
-- ================================================================

CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) NOT NULL,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity DECIMAL(18,2) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_OrderItems PRIMARY KEY CLUSTERED (OrderItemId ASC),
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) 
        REFERENCES Orders(OrderId) 
        ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) 
        REFERENCES Products(ProductId),
    CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_OrderItems_Price CHECK (Price >= 0),
    CONSTRAINT CK_OrderItems_LineTotal CHECK (LineTotal >= 0)
);
GO

PRINT 'OrderItems table created successfully.';
GO

-- ================================================================
-- Create Indexes for Performance
-- ================================================================

-- Index on Products Code (frequently queried)
CREATE NONCLUSTERED INDEX IX_Products_Code 
    ON Products(Code);
GO

-- Index on Customers Code (frequently queried)
CREATE NONCLUSTERED INDEX IX_Customers_Code 
    ON Customers(Code);
GO

-- Index on Orders OrderNumber (frequently queried)
CREATE NONCLUSTERED INDEX IX_Orders_OrderNumber 
    ON Orders(OrderNumber);
GO

-- Index on Orders CustomerId (for joins)
CREATE NONCLUSTERED INDEX IX_Orders_CustomerId 
    ON Orders(CustomerId);
GO

-- Index on Orders Status (for filtering)
CREATE NONCLUSTERED INDEX IX_Orders_Status 
    ON Orders(Status);
GO

-- Index on OrderItems OrderId (for joins)
CREATE NONCLUSTERED INDEX IX_OrderItems_OrderId 
    ON OrderItems(OrderId);
GO

-- Index on OrderItems ProductId (for joins)
CREATE NONCLUSTERED INDEX IX_OrderItems_ProductId 
    ON OrderItems(ProductId);
GO

PRINT 'Indexes created successfully.';
GO

-- ================================================================
-- Verification Query
-- ================================================================
SELECT 
    'Database created successfully!' AS Message,
    DB_NAME() AS DatabaseName,
    (SELECT COUNT(*) FROM sys.tables WHERE type = 'U') AS TableCount;
GO

-- List all tables
SELECT 
    TABLE_NAME AS TableName,
    (SELECT COUNT(*) 
     FROM INFORMATION_SCHEMA.COLUMNS 
     WHERE TABLE_NAME = t.TABLE_NAME) AS ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO

PRINT '==============================================';
PRINT 'Database setup completed successfully!';
PRINT '==============================================';
PRINT 'Database: OrderManagement';
PRINT 'Tables created: 4';
PRINT '  - Products';
PRINT '  - Customers';
PRINT '  - Orders';
PRINT '  - OrderItems';
PRINT '==============================================';
GO

