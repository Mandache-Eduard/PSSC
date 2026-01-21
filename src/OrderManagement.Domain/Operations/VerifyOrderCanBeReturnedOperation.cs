using OrderManagement.Domain.Models;
using System;
using System.Linq;
using static OrderManagement.Domain.Models.ReturnOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class VerifyOrderCanBeReturnedOperation : ReturnOrderOperation
    {
        private readonly Func<OrderNumber, (bool exists, OrderDetails? details)> checkOrderExists;
        public VerifyOrderCanBeReturnedOperation(Func<OrderNumber, (bool, OrderDetails?)> checkOrderExists)
        {
            this.checkOrderExists = checkOrderExists;
        }
        protected override IReturnOrderRequest OnValidated(ValidatedReturnRequest request)
        {
            // Check if order exists
            var (exists, orderDetails) = checkOrderExists(request.OrderNumber);
            if (!exists || orderDetails == null)
            {
                return new InvalidReturnRequest(
                    request.OrderNumber.Value,
                    new[] { $"Order {request.OrderNumber} not found or does not exist" }
                );
            }
            // Business rule 1: Only orders with status "Confirmed" can be returned
            // Cancelled orders cannot be returned
            if (orderDetails.Status != "Confirmed")
            {
                return new InvalidReturnRequest(
                    request.OrderNumber.Value,
                    new[] { $"Order {request.OrderNumber} cannot be returned. Current status: {orderDetails.Status}. Only confirmed orders can be returned." }
                );
            }
            // Business rule 2: Returns must be requested within 14 days of order placement
            TimeSpan timeSinceOrder = DateTime.Now - orderDetails.OrderDate;
            if (timeSinceOrder.TotalDays > 14)
            {
                return new InvalidReturnRequest(
                    request.OrderNumber.Value,
                    new[] { $"Return window expired. Orders can only be returned within 14 days of placement. This order was placed {timeSinceOrder.TotalDays:F0} days ago." }
                );
            }
            // Order exists and can be returned
            return new OrderVerifiedReturnRequest(request.OrderNumber, request.ReturnReason, request.ReturnItems, orderDetails);
        }
    }
}