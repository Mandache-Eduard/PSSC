namespace OrderManagement.Domain.Models
{
    public record Quantity
    {
        public decimal Value { get; }
        private Quantity(decimal value)
        {
            Value = value;
        }
        public static bool TryParse(string? quantityString, out Quantity? quantity)
        {
            quantity = null;
            if (!decimal.TryParse(quantityString, out decimal value))
                return false;
            if (value <= 0)
                return false;
            quantity = new Quantity(value);
            return true;
        }
        public override string ToString() => $"{Value:0.##}";
    }
}