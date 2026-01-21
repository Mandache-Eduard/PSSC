namespace ShoppingCart;

public interface IQuantity
{
    decimal GetValue();
}

public record UnitQuantity(int Units) : IQuantity
{
    public decimal GetValue() => Units;
}

public record KilogramQuantity(decimal Kilograms) : IQuantity
{
    public decimal GetValue() => Kilograms;
}

