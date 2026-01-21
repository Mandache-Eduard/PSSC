namespace ShoppingCart.Domain.Models
{
    public record ProductItem(ProductCode Code, string Name, decimal Price, Quantity Quantity)
    {
        public decimal TotalPrice => Price * Quantity.Value;
        public override string ToString() => $"{Name} ({Code}) - Price: {Price:C}, Quantity: {Quantity}, Total: {TotalPrice:C}";
    }
}