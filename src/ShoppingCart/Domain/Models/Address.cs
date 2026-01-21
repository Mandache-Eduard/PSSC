using ShoppingCart.Domain.Exceptions;
namespace ShoppingCart.Domain.Models
{
    public record Address
    {
        public string Street { get; }
        public string City { get; }
        public string PostalCode { get; }
        public string Country { get; }
        public Address(string street, string city, string postalCode, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new InvalidAddressException("Street cannot be empty.");
            if (string.IsNullOrWhiteSpace(city))
                throw new InvalidAddressException("City cannot be empty.");
            if (string.IsNullOrWhiteSpace(postalCode))
                throw new InvalidAddressException("Postal code cannot be empty.");
            if (string.IsNullOrWhiteSpace(country))
                throw new InvalidAddressException("Country cannot be empty.");
            Street = street;
            City = city;
            PostalCode = postalCode;
            Country = country;
        }
        public override string ToString() => $"{Street}, {City}, {PostalCode}, {Country}";
    }
}