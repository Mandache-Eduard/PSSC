using System;
using System.Collections.Generic;
namespace ShoppingCart.Domain.Models
{
    public static class Cart
    {
        public interface ICart { }
        public record EmptyCart : ICart;
        public record UnvalidatedCart(IReadOnlyCollection<ProductItem> Products) : ICart;
        public record ValidatedCart(IReadOnlyCollection<ProductItem> Products, decimal TotalPrice) : ICart;
        public record PaidCart(
            IReadOnlyCollection<ProductItem> Products,
            decimal TotalPrice,
            DateTime PaidDate,
            Address ShippingAddress) : ICart;
    }
}