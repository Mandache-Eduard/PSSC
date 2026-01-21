namespace OrderManagement.Domain.Models
{
    public record UnvalidatedOrderLine(string ProductCode, string Quantity);
    public record ValidatedOrderLine(ProductCode ProductCode, Quantity Quantity);
    public record ProductVerifiedOrderLine(ProductCode ProductCode, string ProductName, decimal Price, Quantity Quantity);
    public record PricedOrderLine(ProductCode ProductCode, string ProductName, decimal Price, Quantity Quantity, decimal LineTotal)
    {
        public static PricedOrderLine FromVerifiedLine(ProductVerifiedOrderLine line)
        {
            decimal total = line.Price * line.Quantity.Value;
            return new PricedOrderLine(line.ProductCode, line.ProductName, line.Price, line.Quantity, total);
        }
    }
}