﻿namespace ShoppingCart;

class Program
{
    static void Main(string[] args)
    {
        var cart = new Cart();
        bool running = true;

        Console.WriteLine("Welcome to the Shopping Cart Application!");

        while (running)
        {
            DisplayMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddProductToCart(cart);
                    break;
                case "2":
                    RemoveProductFromCart(cart);
                    break;
                case "3":
                    cart.DisplayCart();
                    break;
                case "4":
                    running = false;
                    Console.WriteLine("Thank you for using the Shopping Cart Application!");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            if (running)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    static void DisplayMenu()
    {
        Console.WriteLine("\n=== Shopping Cart Menu ===");
        Console.WriteLine("1. Add Product");
        Console.WriteLine("2. Remove Product");
        Console.WriteLine("3. Display Cart");
        Console.WriteLine("4. Exit");
        Console.Write("\nSelect an option: ");
    }

    static void AddProductToCart(Cart cart)
    {
        Console.Write("\nEnter product name: ");
        var name = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Product name cannot be empty.");
            return;
        }

        Console.Write("Enter price per unit or kilogram: $");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
        {
            Console.WriteLine("Invalid price.");
            return;
        }

        Console.WriteLine("Select quantity type:");
        Console.WriteLine("1. Units");
        Console.WriteLine("2. Kilograms");
        Console.Write("Choice: ");
        var quantityType = Console.ReadLine();

        IQuantity quantity;

        switch (quantityType)
        {
            case "1":
                Console.Write("Enter number of units: ");
                if (!int.TryParse(Console.ReadLine(), out int units) || units <= 0)
                {
                    Console.WriteLine("Invalid unit quantity.");
                    return;
                }
                quantity = new UnitQuantity(units);
                break;
            case "2":
                Console.Write("Enter weight in kilograms: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal kg) || kg <= 0)
                {
                    Console.WriteLine("Invalid kilogram quantity.");
                    return;
                }
                quantity = new KilogramQuantity(kg);
                break;
            default:
                Console.WriteLine("Invalid quantity type.");
                return;
        }

        var product = new Product(name, price, quantity);
        cart.AddProduct(product);
    }

    static void RemoveProductFromCart(Cart cart)
    {
        if (cart.Products.Count == 0)
        {
            Console.WriteLine("\nYour cart is empty. Nothing to remove.");
            return;
        }

        cart.DisplayCart();
        Console.Write("\nEnter the number of the product to remove: ");
        
        if (!int.TryParse(Console.ReadLine(), out int index))
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        cart.RemoveProduct(index - 1);
    }
}

