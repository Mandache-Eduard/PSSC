using System;
namespace ShoppingCart.Domain.Models
{
    public class Customer
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public Address? ShippingAddress { get; private set; }
        public Customer(string name, string email)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
        public void UpdateShippingAddress(Address address)
        {
            ShippingAddress = address;
        }
        public void UpdateName(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        public override string ToString() => $"Customer: {Name} ({Email}) - ID: {Id}";
    }
}