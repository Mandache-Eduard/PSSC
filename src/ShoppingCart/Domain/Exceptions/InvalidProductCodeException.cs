using System;
namespace ShoppingCart.Domain.Exceptions
{
    public class InvalidProductCodeException : Exception
    {
        public InvalidProductCodeException() { }
        public InvalidProductCodeException(string message) : base(message) { }
        public InvalidProductCodeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
