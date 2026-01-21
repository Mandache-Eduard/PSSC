using OrderManagement.Domain.Models;
using System;
using static OrderManagement.Domain.Models.ModifyOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class ProcessModificationOperation : ModifyOrderOperation
    {
        protected override IModifyOrderRequest OnPriceRecalculated(PriceRecalculatedModifyRequest request)
        {
            // Finalize the modification
            // In a real system, this would:
            // - Update the order in the database with new items
            // - Process payment difference (charge more or refund)
            // - Send confirmation email to customer
            // - Update inventory (adjust stock levels)
            // - Generate invoice/receipt
            return new ModifiedOrder(
                request.OrderNumber,
                request.NewOrderLines,
                request.NewTotalPrice,
                request.PriceDifference,
                DateTime.Now
            );
        }
    }
}