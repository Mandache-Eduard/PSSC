using Microsoft.Data.Sqlite;
using System;

namespace DatabaseVerifier
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db";
            
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                
                Console.WriteLine("========================================");
                Console.WriteLine("DATABASE VERIFICATION");
                Console.WriteLine("========================================");
                Console.WriteLine($"Database: {connection.DataSource}");
                Console.WriteLine();
                
                // Count Products
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM Products";
                    var productCount = command.ExecuteScalar();
                    Console.WriteLine($"✓ Products in database: {productCount}");
                }
                
                // Count Customers
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM Customers";
                    var customerCount = command.ExecuteScalar();
                    Console.WriteLine($"✓ Customers in database: {customerCount}");
                }
                
                // Count Orders
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM Orders";
                    var orderCount = command.ExecuteScalar();
                    Console.WriteLine($"✓ Orders in database: {orderCount}");
                }
                
                // Count OrderItems
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM OrderItems";
                    var itemCount = command.ExecuteScalar();
                    Console.WriteLine($"✓ OrderItems in database: {itemCount}");
                }
                
                Console.WriteLine();
                Console.WriteLine("--- Sample Products ---");
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Code, Name, Price, Stock FROM Products LIMIT 5";
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"  {reader.GetString(0)} - {reader.GetString(1)} (${reader.GetDecimal(2):F2}) - Stock: {reader.GetInt32(3)}");
                    }
                }
                
                Console.WriteLine();
                Console.WriteLine("--- Sample Customers ---");
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Code, Name, Email FROM Customers LIMIT 5";
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"  {reader.GetString(0)} - {reader.GetString(1)} ({reader.GetString(2)})");
                    }
                }
                
                Console.WriteLine();
                Console.WriteLine("========================================");
                Console.WriteLine("✓ DATABASE VERIFICATION SUCCESSFUL!");
                Console.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }
        }
    }
}

