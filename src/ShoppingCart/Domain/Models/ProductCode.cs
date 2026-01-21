using ShoppingCart.Domain.Exceptions;
using System.Text.RegularExpressions;
namespace ShoppingCart.Domain.Models
{
    public record ProductCode
    {
        private static readonly Regex ValidPattern = new("^[A-Z]{2}[0-9]{4}$");
        public string Value { get; }
        public ProductCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidProductCodeException("Product code cannot be empty.");
            if (!ValidPattern.IsMatch(value))
                throw new InvalidProductCodeException($"{value} is invalid. Format: XX1234");
            Value = value;
        }
        public override string ToString() => Value;
    }
}