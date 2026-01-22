using OrderManagement.Data.Models;

namespace OrderManagement.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(OrderManagementContext context)
        {
            // Create database if it doesn't exist
            context.Database.EnsureCreated();

            // Check if already seeded
            if (context.Products.Any())
            {
                return; // Database has been seeded
            }

            // Seed Products
            var products = new ProductDto[]
            {
                new ProductDto { Code = "AB1234", Name = "Laptop", Price = 999.99m, Stock = 10, IsActive = true },
                new ProductDto { Code = "CD5678", Name = "Mouse", Price = 29.99m, Stock = 50, IsActive = true },
                new ProductDto { Code = "EF9012", Name = "Keyboard", Price = 79.99m, Stock = 25, IsActive = true },
                new ProductDto { Code = "GH3456", Name = "Monitor", Price = 299.99m, Stock = 5, IsActive = true },
                new ProductDto { Code = "IJ7890", Name = "Headphones", Price = 149.99m, Stock = 15, IsActive = true },
                new ProductDto { Code = "KL1357", Name = "Webcam", Price = 89.99m, Stock = 20, IsActive = true },
                new ProductDto { Code = "MN2468", Name = "USB Cable", Price = 12.99m, Stock = 100, IsActive = true },
                new ProductDto { Code = "OP3579", Name = "Mouse Pad", Price = 9.99m, Stock = 75, IsActive = true },
                new ProductDto { Code = "QR4680", Name = "Laptop Stand", Price = 49.99m, Stock = 30, IsActive = true },
                new ProductDto { Code = "ST5791", Name = "External SSD 1TB", Price = 199.99m, Stock = 12, IsActive = true }
            };
            context.Products.AddRange(products);

            // Seed Customers
            var customers = new CustomerDto[]
            {
                new CustomerDto { Code = "CUST001", Name = "John Doe", Email = "john.doe@email.com", IsActive = true },
                new CustomerDto { Code = "CUST002", Name = "Jane Smith", Email = "jane.smith@email.com", IsActive = true },
                new CustomerDto { Code = "CUST003", Name = "Bob Johnson", Email = "bob.johnson@email.com", IsActive = true },
                new CustomerDto { Code = "CUST004", Name = "Alice Williams", Email = "alice.williams@email.com", IsActive = true },
                new CustomerDto { Code = "CUST005", Name = "Charlie Brown", Email = "charlie.brown@email.com", IsActive = true }
            };
            context.Customers.AddRange(customers);

            context.SaveChanges();
        }
    }
}

