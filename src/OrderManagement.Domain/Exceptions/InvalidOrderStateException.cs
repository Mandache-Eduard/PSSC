using System;
namespace OrderManagement.Domain.Exceptions
{
    public class InvalidOrderStateException : Exception
    {
        public InvalidOrderStateException(string state) 
            : base($"Invalid order state: {state}") { }
    }
}