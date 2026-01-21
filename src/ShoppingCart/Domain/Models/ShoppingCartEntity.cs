using System;
using System.Collections.Generic;
using System.Linq;
using static ShoppingCart.Domain.Models.Cart;
namespace ShoppingCart.Domain.Models
{
    public class ShoppingCartEntity
    {
        public Guid Id { get; }
        public Customer Customer { get; }
        public ICart CurrentState { get; private set; }
        public ShoppingCartEntity(Customer customer)
        {
            Id = Guid.NewGuid();
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            CurrentState = new EmptyCart();
        }
        public void AddProduct(ProductItem product)
        {
            var products = CurrentState switch
            {
                EmptyCart => new List<ProductItem> { product },
                UnvalidatedCart unvalidated => new List<ProductItem>(unvalidated.Products) { product },
                _ => throw new InvalidOperationException($"Cannot add products to cart in {CurrentState.GetType().Name} state")
            };
            CurrentState = new UnvalidatedCart(products);
        }
        public void ValidateCart()
        {
            if (CurrentState is UnvalidatedCart unvalidated)
            {
                decimal total = unvalidated.Products.Sum(p => (decimal)p.TotalPrice);
                CurrentState = new ValidatedCart(unvalidated.Products, total);
            }
            else
            {
                throw new InvalidOperationException($"Cannot validate cart in {CurrentState.GetType().Name} state");
            }
        }
        public void ProcessPayment(Address shippingAddress)
        {
            if (CurrentState is ValidatedCart validated)
            {
                CurrentState = new PaidCart(validated.Products, validated.TotalPrice, DateTime.Now, shippingAddress);
            }
            else
            {
                throw new InvalidOperationException($"Cannot process payment for cart in {CurrentState.GetType().Name} state");
            }
        }
        public string GetCartStateDescription() => CurrentState switch
        {
            EmptyCart => "Cart is empty - ready to add products",
            UnvalidatedCart unvalidated => $"Cart has {unvalidated.Products.Count} product(s) - not validated yet",
            ValidatedCart validated => $"Cart is validated with total: {validated.TotalPrice:C} - ready for payment",
            PaidCart paid => $"Cart was paid on {paid.PaidDate:g} - Total: {paid.TotalPrice:C}",
            _ => "Unknown cart state"
        };
        public override string ToString() => $"Shopping Cart {Id} for {Customer.Name} - State: {CurrentState.GetType().Name}";
    }
}