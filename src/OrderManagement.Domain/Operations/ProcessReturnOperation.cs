using OrderManagement.Domain.Models;
using System;
using static OrderManagement.Domain.Models.ReturnOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class ProcessReturnOperation : ReturnOrderOperation
    {
        protected override IReturnOrderRequest OnReturnApproved(ReturnApprovedRequest request)
        {
            // Generate unique return number
            string returnNumber = $"RET-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            // Finalize the return
            // In a real system, this would:
            // - Update order status to "Returned"
            // - Process refund through payment gateway
            // - Send return label/instructions via email
            // - Update inventory (return items to stock)
            // - Create return tracking record
            return new ProcessedReturn(
                request.OrderNumber,
                request.ReturnReason,
                request.ReturnItems,
                request.RefundAmount,
                request.ShippingFee,
                DateTime.Now,
                returnNumber
            );
        }
    }
}