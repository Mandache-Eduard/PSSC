namespace ShoppingCart;

public record Product(string Name, decimal PricePerUnit, IQuantity Quantity)
{
    public decimal CalculateTotalPrice()
    {
        return Quantity switch
        {
            UnitQuantity unitQty => PricePerUnit * unitQty.Units,
            KilogramQuantity kgQty => PricePerUnit * kgQty.Kilograms,
            _ => 0m
        };
    }

    public override string ToString()
    {
        var quantityStr = Quantity switch
        {
            UnitQuantity unitQty => $"{unitQty.Units} unit(s)",
            KilogramQuantity kgQty => $"{kgQty.Kilograms:F2} kg",
            _ => "unknown"
        };
        
        return $"{Name} - {quantityStr} @ ${PricePerUnit:F2} = ${CalculateTotalPrice():F2}";
    }
}

