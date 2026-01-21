using System.Collections.Generic;
namespace OrderManagement.Domain.Models
{
    public static class Order
    {
        public interface IOrder { }
        public record UnvalidatedOrder : IOrder
        {
            public UnvalidatedOrder(IReadOnlyCollection<UnvalidatedOrderLine> orderLines, string street, string city, string postalCode, string country)
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
        public record InvalidOrder : IOrder
        {
            internal InvalidOrder(IReadOnlyCollection<UnvalidatedOrderLine> orderLines, IEnumerable<string> reasons)
            {
                OrderLines = orderLines;
                Reasons = reasons;
            }
            public IReadOnlyCollection<UnvalidatedOrderLine> OrderLines { get; }
            public IEnumerable<string> Reasons { get; }
        }
        public record ValidatedOrder : IOrder
        {
            internal ValidatedOrder(IReadOnlyCollection<ValidatedOrderLine> orderLines, Address shippingAddress)
            {
                OrderLines = orderLines;
                ShippingAddress = shippingAddress;
            }
            public IReadOnlyCollection<ValidatedOrderLine> OrderLines { get; }
            public Address ShippingAddress { get; }
        }
        public record ProductVerifiedOrder : IOrder
        {
            internal ProductVerifiedOrder(IReadOnlyCollection<ProductVerifiedOrderLine> orderLines, Address shippingAddress)
            {
                OrderLines = orderLines;
                ShippingAddress = shippingAddress;
            }
            public IReadOnlyCollection<ProductVerifiedOrderLine> OrderLines { get; }
            public Address ShippingAddress { get; }
        }
        public record PricedOrder : IOrder
        {
            internal PricedOrder(IReadOnlyCollection<PricedOrderLine> orderLines, Address shippingAddress, decimal totalPrice)
            {
                OrderLines = orderLines;
                ShippingAddress = shippingAddress;
                TotalPrice = totalPrice;
            }
            public IReadOnlyCollection<PricedOrderLine> OrderLines { get; }
            public Address ShippingAddress { get; }
            public decimal TotalPrice { get; }
        }
        public record ConfirmedOrder : IOrder
        {
            internal ConfirmedOrder(IReadOnlyCollection<PricedOrderLine> orderLines, Address shippingAddress, decimal totalPrice, string orderNumber)
            {
                OrderLines = orderLines;
                ShippingAddress = shippingAddress;
                TotalPrice = totalPrice;
                OrderNumber = orderNumber;
            }
            public IReadOnlyCollection<PricedOrderLine> OrderLines { get; }
            public Address ShippingAddress { get; }
            public decimal TotalPrice { get; }
            public string OrderNumber { get; }
        }
    }
}