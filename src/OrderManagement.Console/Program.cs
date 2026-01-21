using OrderManagement.Domain.Models;
using OrderManagement.Domain.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using static OrderManagement.Domain.Models.OrderPlacedEvent;
using static OrderManagement.Domain.Models.OrderCancelledEvent;
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
        // Simulated order database (stores placed orders)
        private static readonly Dictionary<string, OrderDetails> Orders = new();
        static void Main(string[] args)
        {
            while (true)
            {
                System.Console.Clear();
                System.Console.WriteLine("=== Order Management System ===\n");
                System.Console.WriteLine("1. Place New Order");
                System.Console.WriteLine("2. Cancel Existing Order");
                System.Console.WriteLine("3. View Placed Orders");
                System.Console.WriteLine("4. Exit");
                System.Console.Write("\nChoose an option: ");
                string? choice = System.Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        PlaceNewOrder();
                        break;
                    case "2":
                        CancelExistingOrder();
                        break;
                    case "3":
                        ViewPlacedOrders();
                        break;
                    case "4":
                        System.Console.WriteLine("\nThank you for using Order Management System!");
                        return;
                    default:
                        System.Console.WriteLine("\nInvalid option. Press any key to continue...");
                        System.Console.ReadKey();
                        break;
                }
            }
        }
        private static void PlaceNewOrder()
        {
            System.Console.Clear();
            System.Console.WriteLine("=== PLACE NEW ORDER ===\n");
            System.Console.WriteLine("Available Products:");
            System.Console.WriteLine("  AB1234 - Laptop ($999.99) - Stock: 10");
            System.Console.WriteLine("  CD5678 - Mouse ($29.99) - Stock: 50");
            System.Console.WriteLine("  EF9012 - Keyboard ($79.99) - Stock: 25");
            System.Console.WriteLine("  GH3456 - Monitor ($299.99) - Stock: 5");
            System.Console.WriteLine("  IJ7890 - Headphones ($149.99) - Stock: 15");
            System.Console.WriteLine();
            // Read order lines
            List<UnvalidatedOrderLine> orderLines = ReadOrderLines();
            if (orderLines.Count == 0)
            {
                System.Console.WriteLine("\nNo products added. Returning to menu...");
                System.Console.ReadKey();
                return;
            }
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
            // Create command and execute workflow
            PlaceOrderCommand command = new(orderLines, street!, city!, postalCode!, country!);
            PlaceOrderWorkflow workflow = new();
            IOrderPlacedEvent result = workflow.Execute(command, CheckProductCatalog, CheckInventory);
            // Display result
            System.Console.WriteLine("\n========================================");
            System.Console.WriteLine("ORDER RESULT:");
            System.Console.WriteLine("========================================");
            string message = result switch
            {
                OrderPlacedSuccessEvent successEvent => HandlePlaceOrderSuccess(successEvent),
                OrderPlacedFailedEvent failedEvent => FormatPlaceOrderFailure(failedEvent),
                _ => throw new NotImplementedException()
            };
            System.Console.WriteLine(message);
            System.Console.WriteLine("\nPress any key to continue...");
            System.Console.ReadKey();
        }
        private static void CancelExistingOrder()
        {
            System.Console.Clear();
            System.Console.WriteLine("=== CANCEL ORDER ===\n");
            if (Orders.Count == 0)
            {
                System.Console.WriteLine("No orders available to cancel.");
                System.Console.WriteLine("\nPress any key to continue...");
                System.Console.ReadKey();
                return;
            }
            System.Console.WriteLine("Available orders for cancellation:");
            foreach (var order in Orders.Where(o => o.Value.Status == "Confirmed"))
            {
                System.Console.WriteLine($"  {order.Key} - Total: {order.Value.TotalAmount:C} - Status: {order.Value.Status} - Placed: {order.Value.OrderDate:g}");
            }
            System.Console.WriteLine();
            System.Console.Write("Enter Order Number to cancel: ");
            string? orderNumber = System.Console.ReadLine();
            System.Console.Write("Enter Cancellation Reason (min 10 characters): ");
            string? reason = System.Console.ReadLine();
            // Create command and execute workflow
            CancelOrderCommand command = new(orderNumber!, reason!);
            CancelOrderWorkflow workflow = new();
            IOrderCancelledEvent result = workflow.Execute(command, CheckOrderExists);
            // Display result
            System.Console.WriteLine("\n========================================");
            System.Console.WriteLine("CANCELLATION RESULT:");
            System.Console.WriteLine("========================================");
            string message = result switch
            {
                OrderCancelledSuccessEvent successEvent => HandleCancelOrderSuccess(successEvent),
                OrderCancelledFailedEvent failedEvent => FormatCancelOrderFailure(failedEvent),
                _ => throw new NotImplementedException()
            };
            System.Console.WriteLine(message);
            System.Console.WriteLine("\nPress any key to continue...");
            System.Console.ReadKey();
        }
        private static void ViewPlacedOrders()
        {
            System.Console.Clear();
            System.Console.WriteLine("=== PLACED ORDERS ===\n");
            if (Orders.Count == 0)
            {
                System.Console.WriteLine("No orders have been placed yet.");
            }
            else
            {
                foreach (var order in Orders)
                {
                    System.Console.WriteLine($"Order: {order.Key}");
                    System.Console.WriteLine($"  Total: {order.Value.TotalAmount:C}");
                    System.Console.WriteLine($"  Status: {order.Value.Status}");
                    System.Console.WriteLine($"  Date: {order.Value.OrderDate:g}");
                    System.Console.WriteLine();
                }
            }
            System.Console.WriteLine("Press any key to continue...");
            System.Console.ReadKey();
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
        private static (bool exists, OrderDetails? details) CheckOrderExists(OrderNumber orderNumber)
        {
            if (Orders.TryGetValue(orderNumber.Value, out var orderDetails))
            {
                return (true, orderDetails);
            }
            return (false, null);
        }
        private static string HandlePlaceOrderSuccess(OrderPlacedSuccessEvent @event)
        {
            // Store the order in our simulated database
            Orders[@event.OrderNumber] = new OrderDetails(
                @event.TotalPrice,
                @event.PlacedDate,
                "Confirmed"
            );
            return $@"✓ ORDER PLACED SUCCESSFULLY!
{@event.Summary}
Order placed on: {@event.PlacedDate:g}";
        }
        private static string HandleCancelOrderSuccess(OrderCancelledSuccessEvent @event)
        {
            // Update order status in our simulated database
            if (Orders.ContainsKey(@event.OrderNumber))
            {
                var order = Orders[@event.OrderNumber];
                Orders[@event.OrderNumber] = new OrderDetails(
                    order.TotalAmount,
                    order.OrderDate,
                    "Cancelled"
                );
            }
            return $@"✓ ORDER CANCELLED SUCCESSFULLY!
{@event.Summary}";
        }
        private static string FormatPlaceOrderFailure(OrderPlacedFailedEvent @event)
        {
            return $@"✗ ORDER FAILED
The following errors occurred:
{string.Join("\n", @event.Reasons.Select(r => $"  • {r}"))}
Please correct the errors and try again.";
        }
        private static string FormatCancelOrderFailure(OrderCancelledFailedEvent @event)
        {
            return $@"✗ CANCELLATION FAILED
Error: {@event.Reason}
Please check the information and try again.";
        }
    }
}