using OrderManagement.Domain.Models;
using System;
using static OrderManagement.Domain.Models.CancelOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class VerifyOrderExistsOperation : CancelOrderOperation
    {
        private readonly Func<OrderNumber, (bool exists, OrderDetails? details)> checkOrderExists;
        public VerifyOrderExistsOperation(Func<OrderNumber, (bool, OrderDetails?)> checkOrderExists)
        {
            this.checkOrderExists = checkOrderExists;
        }
        protected override ICancelOrderRequest OnValidated(ValidatedCancelRequest request)
        {
            // Check if order exists
            var (exists, orderDetails) = checkOrderExists(request.OrderNumber);
            if (!exists || orderDetails == null)
            {
                return new InvalidCancelRequest(
                    request.OrderNumber.Value,
                    request.Reason.Value,
                    $"Order {request.OrderNumber} not found or does not exist"
                );
            }
            // Check if order can be cancelled (business rule: only orders with status "Confirmed" can be cancelled)
            if (orderDetails.Status != "Confirmed")
            {
                return new InvalidCancelRequest(
                    request.OrderNumber.Value,
                    request.Reason.Value,
                    $"Order {request.OrderNumber} cannot be cancelled. Current status: {orderDetails.Status}. Only confirmed orders can be cancelled."
                );
            }
            // Order exists and can be cancelled
            return new OrderVerifiedCancelRequest(request.OrderNumber, request.Reason, orderDetails);
        }
    }
}