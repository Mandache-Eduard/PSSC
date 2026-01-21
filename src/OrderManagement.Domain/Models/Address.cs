namespace OrderManagement.Domain.Models
{
    public record Address
    {
        public string Street { get; }
        public string City { get; }
        public string PostalCode { get; }
        public string Country { get; }
        private Address(string street, string city, string postalCode, string country)
        {
            Street = street;
            City = city;
            PostalCode = postalCode;
            Country = country;
        }
        public static bool TryParse(string? street, string? city, string? postalCode, string? country, out Address? address)
        {
            address = null;
            if (string.IsNullOrWhiteSpace(street) || 
                string.IsNullOrWhiteSpace(city) || 
                string.IsNullOrWhiteSpace(postalCode) || 
                string.IsNullOrWhiteSpace(country))
                return false;
            address = new Address(street, city, postalCode, country);
            return true;
        }
        public override string ToString() => $"{Street}, {City}, {PostalCode}, {Country}";
    }
}