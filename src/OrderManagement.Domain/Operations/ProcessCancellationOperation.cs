using OrderManagement.Domain.Models;
using System;
using static OrderManagement.Domain.Models.CancelOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class ProcessCancellationOperation : CancelOrderOperation
    {
        protected override ICancelOrderRequest OnRefundCalculated(RefundCalculatedCancelRequest request)
        {
            // Finalize the cancellation
            // In a real system, this would:
            // - Update the order status in the database
            // - Process the refund through payment gateway
            // - Send confirmation email to customer
            // - Update inventory (return items to stock)
            return new CancelledOrder(
                request.OrderNumber,
                request.Reason,
                request.OrderDetails,
                request.RefundAmount,
                DateTime.Now
            );
        }
    }
}