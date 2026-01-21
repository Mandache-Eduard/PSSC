using OrderManagement.Domain.Models;
using System;
using System.Linq;
using static OrderManagement.Domain.Models.ModifyOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class VerifyOrderCanBeModifiedOperation : ModifyOrderOperation
    {
        private readonly Func<OrderNumber, (bool exists, OrderDetails? details)> checkOrderExists;
        public VerifyOrderCanBeModifiedOperation(Func<OrderNumber, (bool, OrderDetails?)> checkOrderExists)
        {
            this.checkOrderExists = checkOrderExists;
        }
        protected override IModifyOrderRequest OnValidated(ValidatedModifyRequest request)
        {
            // Check if order exists
            var (exists, orderDetails) = checkOrderExists(request.OrderNumber);
            if (!exists || orderDetails == null)
            {
                var unvalidatedLines = request.NewOrderLines.Select(l =>
                    new UnvalidatedOrderLine(l.ProductCode.Value, l.Quantity.Value.ToString())).ToList();
                return new InvalidModifyRequest(
                    request.OrderNumber.Value,
                    new[] { $"Order {request.OrderNumber} not found or does not exist" }
                );
            }
            // Business rule: Only orders with status "Confirmed" can be modified
            // Orders that are "Shipped", "Delivered", or "Cancelled" cannot be changed
            if (orderDetails.Status != "Confirmed")
            {
                var unvalidatedLines = request.NewOrderLines.Select(l =>
                    new UnvalidatedOrderLine(l.ProductCode.Value, l.Quantity.Value.ToString())).ToList();
                return new InvalidModifyRequest(
                    request.OrderNumber.Value,
                    new[] { $"Order {request.OrderNumber} cannot be modified. Current status: {orderDetails.Status}. Only confirmed orders can be modified." }
                );
            }
            // Business rule: Orders can only be modified within 24 hours of placement
            TimeSpan timeSinceOrder = DateTime.Now - orderDetails.OrderDate;
            if (timeSinceOrder.TotalHours > 24)
            {
                var unvalidatedLines = request.NewOrderLines.Select(l =>
                    new UnvalidatedOrderLine(l.ProductCode.Value, l.Quantity.Value.ToString())).ToList();
                return new InvalidModifyRequest(
                    request.OrderNumber.Value,
                    new[] { $"Order {request.OrderNumber} cannot be modified. Orders can only be modified within 24 hours of placement. This order was placed {timeSinceOrder.TotalHours:F1} hours ago." }
                );
            }
            // Order exists and can be modified
            return new OrderVerifiedModifyRequest(request.OrderNumber, request.NewOrderLines, orderDetails);
        }
    }
}