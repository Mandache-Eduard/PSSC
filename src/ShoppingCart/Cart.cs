namespace ShoppingCart;

public record Cart(List<Product> Products)
{
    public Cart() : this(new List<Product>()) { }

    public void AddProduct(Product product)
    {
        Products.Add(product);
        Console.WriteLine($"Added: {product.Name}");
    }

    public void RemoveProduct(int index)
    {
        if (index >= 0 && index < Products.Count)
        {
            var product = Products[index];
            Products.RemoveAt(index);
            Console.WriteLine($"Removed: {product.Name}");
        }
        else
        {
            Console.WriteLine("Invalid product index.");
        }
    }

    public void DisplayCart()
    {
        if (Products.Count == 0)
        {
            Console.WriteLine("\nYour shopping cart is empty.");
            return;
        }

        Console.WriteLine("\n=== Shopping Cart ===");
        for (int i = 0; i < Products.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Products[i]}");
        }
        Console.WriteLine($"\nTotal Price: ${CalculateTotalPrice():F2}");
        Console.WriteLine("====================");
    }

    public decimal CalculateTotalPrice()
    {
        return Products.Sum(product => product.CalculateTotalPrice());
    }
}

