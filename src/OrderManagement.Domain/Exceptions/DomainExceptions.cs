using System;
namespace OrderManagement.Domain.Exceptions
{
    public class InvalidProductCodeException : Exception
    {
        public InvalidProductCodeException() { }
        public InvalidProductCodeException(string message) : base(message) { }
    }
    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException() { }
        public InvalidQuantityException(string message) : base(message) { }
    }
    public class InvalidAddressException : Exception
    {
        public InvalidAddressException() { }
        public InvalidAddressException(string message) : base(message) { }
    }
}