using OrderManagement.Domain.Exceptions;
using System.Text.RegularExpressions;
namespace OrderManagement.Domain.Models
{
    public record ProductCode
    {
        private static readonly Regex ValidPattern = new("^[A-Z]{2}[0-9]{4}$");
        public string Value { get; }
        private ProductCode(string value)
        {
            Value = value;
        }
        public static bool TryParse(string? productCodeString, out ProductCode? productCode)
        {
            productCode = null;
            if (string.IsNullOrWhiteSpace(productCodeString))
                return false;
            if (!ValidPattern.IsMatch(productCodeString))
                return false;
            productCode = new ProductCode(productCodeString);
            return true;
        }
        public override string ToString() => Value;
    }
}