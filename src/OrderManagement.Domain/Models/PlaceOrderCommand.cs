using System.Collections.Generic;
namespace OrderManagement.Domain.Models
{
    public record PlaceOrderCommand
    {
        public PlaceOrderCommand(IReadOnlyCollection<UnvalidatedOrderLine> orderLines, string street, string city, string postalCode, string country)
        {
            OrderLines = orderLines;
            Street = street;
            City = city;
            PostalCode = postalCode;
            Country = country;
        }
        public IReadOnlyCollection<UnvalidatedOrderLine> OrderLines { get; }
        public string Street { get; }
        public string City { get; }
        public string PostalCode { get; }
        public string Country { get; }
    }
}