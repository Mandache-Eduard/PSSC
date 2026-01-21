using OrderManagement.Domain.Models;
using OrderManagement.Domain.Workflows;
using System;
using System.Collections.Generic;
using static OrderManagement.Domain.Models.OrderPlacedEvent;
namespace OrderManagement.Console
{
    internal class Program
    {
        // Simulated product catalog
        private static readonly Dictionary<string, (string name, decimal price)> ProductCatalog = new()
        {
            { "AB1234", ("Laptop", 999.99m) },
            { "CD5678", ("Mouse", 29.99m) },
            { "EF9012", ("Keyboard", 79.99m) },
            { "GH3456", ("Monitor", 299.99m) },
            { "IJ7890", ("Headphones", 149.99m) }
        };
        // Simulated inventory
        private static readonly Dictionary<string, int> Inventory = new()
        {
            { "AB1234", 10 },
            { "CD5678", 50 },
            { "EF9012", 25 },
            { "GH3456", 5 },
            { "IJ7890", 15 }
        };
        static void Main(string[] args)
        {
            System.Console.WriteLine("=== Order Management System ===");
            System.Console.WriteLine("Available Products:");
            System.Console.WriteLine("  AB1234 - Laptop ($999.99) - Stock: 10");
            System.Console.WriteLine("  CD5678 - Mouse ($29.99) - Stock: 50");
            System.Console.WriteLine("  EF9012 - Keyboard ($79.99) - Stock: 25");
            System.Console.WriteLine("  GH3456 - Monitor ($299.99) - Stock: 5");
            System.Console.WriteLine("  IJ7890 - Headphones ($149.99) - Stock: 15");
            System.Console.WriteLine();
            // Read order lines
            List<UnvalidatedOrderLine> orderLines = ReadOrderLines();
            // Read shipping address
            System.Console.WriteLine("\n--- Shipping Address ---");
            System.Console.Write("Street: ");
            string? street = System.Console.ReadLine();
            System.Console.Write("City: ");
            string? city = System.Console.ReadLine();
            System.Console.Write("Postal Code: ");
            string? postalCode = System.Console.ReadLine();
            System.Console.Write("Country: ");
            string? country = System.Console.ReadLine();
            // Create command
            PlaceOrderCommand command = new(orderLines, street!, city!, postalCode!, country!);
            // Execute workflow
            PlaceOrderWorkflow workflow = new();
            IOrderPlacedEvent result = workflow.Execute(
                command,
                CheckProductCatalog,
                CheckInventory
            );
            // Display result
            System.Console.WriteLine();
            System.Console.WriteLine("========================================");
            System.Console.WriteLine("ORDER RESULT:");
            System.Console.WriteLine("========================================");
            string message = result switch
            {
                OrderPlacedSuccessEvent successEvent => FormatSuccessMessage(successEvent),
                OrderPlacedFailedEvent failedEvent => FormatFailureMessage(failedEvent),
                _ => throw new NotImplementedException()
            };
            System.Console.WriteLine(message);
        }
        private static List<UnvalidatedOrderLine> ReadOrderLines()
        {
            List<UnvalidatedOrderLine> lines = new();
            System.Console.WriteLine("Enter order lines (press Enter with empty product code to finish):");
            while (true)
            {
                System.Console.Write("\nProduct Code: ");
                string? productCode = System.Console.ReadLine();
                if (string.IsNullOrEmpty(productCode))
                    break;
                System.Console.Write("Quantity: ");
                string? quantity = System.Console.ReadLine();
                lines.Add(new UnvalidatedOrderLine(productCode, quantity!));
            }
            return lines;
        }
        private static (bool exists, string productName, decimal price) CheckProductCatalog(ProductCode productCode)
        {
            if (ProductCatalog.TryGetValue(productCode.Value, out var product))
            {
                return (true, product.name, product.price);
            }
            return (false, string.Empty, 0);
        }
        private static bool CheckInventory(ProductCode productCode, Quantity quantity)
        {
            if (Inventory.TryGetValue(productCode.Value, out int stock))
            {
                return stock >= quantity.Value;
            }
            return false;
        }
        private static string FormatSuccessMessage(OrderPlacedSuccessEvent @event)
        {
            return $@"✓ ORDER PLACED SUCCESSFULLY!
{@event.Summary}
Order placed on: {@event.PlacedDate:g}
";
        }
        private static string FormatFailureMessage(OrderPlacedFailedEvent @event)
        {
            return $@"✗ ORDER FAILED
The following errors occurred:
{string.Join("\n", @event.Reasons.Select(r => $"  • {r}"))}
Please correct the errors and try again.
";
        }
    }
}