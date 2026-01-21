using ShoppingCart.Domain.Models;
using System;
using System.Linq;
using static ShoppingCart.Domain.Models.Cart;
namespace ShoppingCart
{
    internal class Program
    {
        private static ShoppingCartEntity? _cart;
        private static Customer? _currentCustomer;
        static void Main(string[] args)
        {
            Console.WriteLine("=== Shopping Cart Application ===\n");
            InitializeCustomer();
            _cart = new ShoppingCartEntity(_currentCustomer!);
            Console.WriteLine($"\n{_cart}");
            Console.WriteLine($"State: {_cart.GetCartStateDescription()}\n");
            bool running = true;
            while (running)
            {
                DisplayMenu();
                string? choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddProductToCart();
                            break;
                        case "2":
                            DisplayCartContents();
                            break;
                        case "3":
                            TransitionCartState();
                            break;
                        case "4":
                            DisplayCustomerInfo();
                            break;
                        case "5":
                            running = false;
                            Console.WriteLine("Thank you for using the Shopping Cart Application!");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                Console.WriteLine();
            }
        }
        private static void InitializeCustomer()
        {
            Console.Write("Enter your name: ");
            string? name = Console.ReadLine();
            Console.Write("Enter your email: ");
            string? email = Console.ReadLine();
            _currentCustomer = new Customer(
                string.IsNullOrWhiteSpace(name) ? "Guest" : name,
                string.IsNullOrWhiteSpace(email) ? "guest@example.com" : email
            );
            Console.WriteLine($"\nWelcome, {_currentCustomer.Name}!");
        }
        private static void DisplayMenu()
        {
            Console.WriteLine("--- Menu ---");
            Console.WriteLine("1. Add Product to Cart");
            Console.WriteLine("2. Display Cart Contents");
            Console.WriteLine("3. Transition Cart State");
            Console.WriteLine("4. Display Customer Info");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
        }
        private static void AddProductToCart()
        {
            Console.WriteLine("\n--- Add Product ---");
            Console.Write("Product Code (format XX1234, e.g., AB1234): ");
            string? codeInput = Console.ReadLine();
            ProductCode code = new ProductCode(codeInput!);
            Console.Write("Product Name: ");
            string? name = Console.ReadLine();
            Console.Write("Price: ");
            decimal price = decimal.Parse(Console.ReadLine()!);
            Console.Write("Quantity: ");
            decimal quantityValue = decimal.Parse(Console.ReadLine()!);
            Quantity quantity = new Quantity(quantityValue);
            ProductItem product = new ProductItem(code, name!, price, quantity);
            _cart!.AddProduct(product);
            Console.WriteLine($"\nProduct added successfully!");
            Console.WriteLine($"Current state: {_cart.GetCartStateDescription()}");
        }
        private static void DisplayCartContents()
        {
            Console.WriteLine("\n--- Cart Contents ---");
            Console.WriteLine($"Cart ID: {_cart!.Id}");
            Console.WriteLine($"State: {_cart.GetCartStateDescription()}");
            Console.WriteLine();
            string cartDetails = _cart.CurrentState switch
            {
                EmptyCart => "The cart is empty.",
                UnvalidatedCart unvalidated => DisplayUnvalidatedCart(unvalidated),
                ValidatedCart validated => DisplayValidatedCart(validated),
                PaidCart paid => DisplayPaidCart(paid),
                _ => "Unknown cart state"
            };
            Console.WriteLine(cartDetails);
        }
        private static string DisplayUnvalidatedCart(UnvalidatedCart cartState)
        {
            var result = "Products in cart (not validated):\n";
            int index = 1;
            foreach (var product in cartState.Products)
            {
                result += $"{index++}. {product}\n";
            }
            decimal total = cartState.Products.Sum(p => (decimal)p.TotalPrice);
            result += $"\nEstimated Total: {total:C}";
            return result;
        }
        private static string DisplayValidatedCart(ValidatedCart cartState)
        {
            var result = "Products in cart (validated):\n";
            int index = 1;
            foreach (var product in cartState.Products)
            {
                result += $"{index++}. {product}\n";
            }
            result += $"\nTotal: {cartState.TotalPrice:C}";
            result += "\nCart is ready for payment!";
            return result;
        }
        private static string DisplayPaidCart(PaidCart cartState)
        {
            var result = "Products in cart (paid):\n";
            int index = 1;
            foreach (var product in cartState.Products)
            {
                result += $"{index++}. {product}\n";
            }
            result += $"\nTotal: {cartState.TotalPrice:C}";
            result += $"\nPaid on: {cartState.PaidDate:g}";
            result += $"\nShipping to: {cartState.ShippingAddress}";
            return result;
        }
        private static void TransitionCartState()
        {
            Console.WriteLine("\n--- Transition Cart State ---");
            Console.WriteLine($"Current state: {_cart!.GetCartStateDescription()}");
            Console.WriteLine();
            var availableTransitions = _cart.CurrentState switch
            {
                EmptyCart => "No transitions available. Please add products first.",
                UnvalidatedCart => "1. Validate Cart",
                ValidatedCart => "1. Process Payment",
                PaidCart => "Cart is already paid. No further transitions.",
                _ => "Unknown state"
            };
            Console.WriteLine("Available transitions:");
            Console.WriteLine(availableTransitions);
            if (_cart.CurrentState is UnvalidatedCart or ValidatedCart)
            {
                Console.Write("\nChoose transition (1): ");
                string? choice = Console.ReadLine();
                if (choice == "1")
                {
                    if (_cart.CurrentState is UnvalidatedCart)
                    {
                        _cart.ValidateCart();
                        Console.WriteLine("Cart validated successfully!");
                    }
                    else if (_cart.CurrentState is ValidatedCart)
                    {
                        ProcessPaymentTransition();
                    }
                }
            }
        }
        private static void ProcessPaymentTransition()
        {
            Console.WriteLine("\n--- Enter Shipping Address ---");
            Console.Write("Street: ");
            string? street = Console.ReadLine();
            Console.Write("City: ");
            string? city = Console.ReadLine();
            Console.Write("Postal Code: ");
            string? postalCode = Console.ReadLine();
            Console.Write("Country: ");
            string? country = Console.ReadLine();
            Address shippingAddress = new Address(street!, city!, postalCode!, country!);
            _currentCustomer!.UpdateShippingAddress(shippingAddress);
            _cart!.ProcessPayment(shippingAddress);
            Console.WriteLine("Payment processed successfully!");
        }
        private static void DisplayCustomerInfo()
        {
            Console.WriteLine("\n--- Customer Information ---");
            Console.WriteLine(_currentCustomer!.ToString());
            if (_currentCustomer.ShippingAddress != null)
            {
                Console.WriteLine($"Shipping Address: {_currentCustomer.ShippingAddress}");
            }
        }
    }
}