using ShoppingCart.Domain.Exceptions;
namespace ShoppingCart.Domain.Models
{
    public record Quantity
    {
        public decimal Value { get; }
        public Quantity(decimal value)
        {
            if (value <= 0)
                throw new InvalidQuantityException($"{value} is invalid. Must be greater than 0.");
            Value = value;
        }
        public override string ToString() => $"{Value:0.##}";
    }
}