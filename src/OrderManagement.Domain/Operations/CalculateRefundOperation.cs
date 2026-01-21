using OrderManagement.Domain.Models;
using System;
using static OrderManagement.Domain.Models.CancelOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class CalculateRefundOperation : CancelOrderOperation
    {
        protected override ICancelOrderRequest OnOrderVerified(OrderVerifiedCancelRequest request)
        {
            // Business rule: Calculate refund based on how long ago the order was placed
            // - Within 24 hours: 100% refund
            // - Within 48 hours: 80% refund
            // - Within 7 days: 50% refund
            // - After 7 days: No refund
            TimeSpan timeSinceOrder = DateTime.Now - request.OrderDetails.OrderDate;
            decimal refundPercentage;
            if (timeSinceOrder.TotalHours <= 24)
            {
                refundPercentage = 1.00m; // 100%
            }
            else if (timeSinceOrder.TotalHours <= 48)
            {
                refundPercentage = 0.80m; // 80%
            }
            else if (timeSinceOrder.TotalDays <= 7)
            {
                refundPercentage = 0.50m; // 50%
            }
            else
            {
                refundPercentage = 0m; // No refund
            }
            decimal refundAmount = request.OrderDetails.TotalAmount * refundPercentage;
            return new RefundCalculatedCancelRequest(
                request.OrderNumber,
                request.Reason,
                request.OrderDetails,
                refundAmount
            );
        }
    }
}