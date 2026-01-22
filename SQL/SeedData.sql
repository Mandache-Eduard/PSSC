-- ================================================================
-- Seed Data Script - Order Management System
-- ================================================================
-- This script inserts sample data for testing
-- ================================================================

USE OrderManagement;
GO

PRINT 'Starting data seeding...';
GO

-- ================================================================
-- Seed Products
-- ================================================================

SET IDENTITY_INSERT Products ON;
GO

INSERT INTO Products (ProductId, Code, Name, Price, Stock, IsActive)
VALUES 
    (1, 'AB1234', 'Laptop', 999.99, 10, 1),
    (2, 'CD5678', 'Mouse', 29.99, 50, 1),
    (3, 'EF9012', 'Keyboard', 79.99, 25, 1),
    (4, 'GH3456', 'Monitor', 299.99, 5, 1),
    (5, 'IJ7890', 'Headphones', 149.99, 15, 1),
    (6, 'KL1357', 'Webcam', 89.99, 20, 1),
    (7, 'MN2468', 'USB Cable', 12.99, 100, 1),
    (8, 'OP3579', 'Mouse Pad', 9.99, 75, 1),
    (9, 'QR4680', 'Laptop Stand', 49.99, 30, 1),
    (10, 'ST5791', 'External SSD 1TB', 199.99, 12, 1);
GO

SET IDENTITY_INSERT Products OFF;
GO

PRINT '10 products inserted successfully.';
GO

-- ================================================================
-- Seed Customers
-- ================================================================

SET IDENTITY_INSERT Customers ON;
GO

INSERT INTO Customers (CustomerId, Code, Name, Email, IsActive)
VALUES 
    (1, 'CUST001', 'John Doe', 'john.doe@email.com', 1),
    (2, 'CUST002', 'Jane Smith', 'jane.smith@email.com', 1),
    (3, 'CUST003', 'Bob Johnson', 'bob.johnson@email.com', 1),
    (4, 'CUST004', 'Alice Williams', 'alice.williams@email.com', 1),
    (5, 'CUST005', 'Charlie Brown', 'charlie.brown@email.com', 1);
GO

SET IDENTITY_INSERT Customers OFF;
GO

PRINT '5 customers inserted successfully.';
GO

-- ================================================================
-- Verification Queries
-- ================================================================

-- Count products
SELECT 
    COUNT(*) AS TotalProducts,
    SUM(Stock) AS TotalStock,
    AVG(Price) AS AveragePrice
FROM Products;
GO

-- List all products
SELECT 
    ProductId,
    Code,
    Name,
    Price,
    Stock
FROM Products
ORDER BY ProductId;
GO

-- Count customers
SELECT COUNT(*) AS TotalCustomers
FROM Customers;
GO

-- List all customers
SELECT 
    CustomerId,
    Code,
    Name,
    Email
FROM Customers
ORDER BY CustomerId;
GO

PRINT '==============================================';
PRINT 'Data seeding completed successfully!';
PRINT '==============================================';
PRINT 'Products inserted: 10';
PRINT 'Customers inserted: 5';
PRINT 'Orders inserted: 0 (to be created by application)';
PRINT '==============================================';
GO

