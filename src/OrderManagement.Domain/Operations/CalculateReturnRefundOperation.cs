using OrderManagement.Domain.Models;
using static OrderManagement.Domain.Models.ReturnOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class CalculateReturnRefundOperation : ReturnOrderOperation
    {
        protected override IReturnOrderRequest OnOrderVerified(OrderVerifiedReturnRequest request)
        {
            // Calculate refund based on return reason type
            // Business rule: Shipping fee depends on return reason
            // Standard shipping fee (flat rate)
            const decimal standardShippingFee = 15.00m;
            
            decimal shippingFee = request.ReturnReason.Type switch
            {
                ReturnReasonType.Defective => 0m,         // No fee for defective items - company responsibility
                ReturnReasonType.WrongItem => 0m,          // No fee for wrong items (our mistake)
                ReturnReasonType.NotAsDescribed => 0m,     // No fee for items not as described
                ReturnReasonType.Changed_Mind => standardShippingFee,    // Customer pays shipping cost
                _ => standardShippingFee
            };
            // Calculate total return amount (from original order)
            // For simplicity, we assume full order total is being returned
            // In a real system, we would calculate based on specific items
            decimal originalAmount = request.OriginalOrderDetails.TotalAmount;
            decimal refundAmount = originalAmount - shippingFee;
            return new ReturnApprovedRequest(
                request.OrderNumber,
                request.ReturnReason,
                request.ReturnItems,
                request.OriginalOrderDetails,
                refundAmount,
                shippingFee
            );
        }
    }
}