using OrderManagement.Domain.Models;
using System.Linq;
using static OrderManagement.Domain.Models.ModifyOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class RecalculatePriceOperation : ModifyOrderOperation
    {
        protected override IModifyOrderRequest OnProductsVerified(ProductsVerifiedModifyRequest request)
        {
            // Calculate new order total using LINQ
            var pricedLines = request.NewOrderLines
                .Select(line => PricedOrderLine.FromVerifiedLine(line))
                .ToList();
            decimal newTotalPrice = pricedLines.Sum(line => line.LineTotal);
            // Calculate price difference (positive = customer pays more, negative = customer gets refund)
            decimal priceDifference = newTotalPrice - request.OriginalOrderDetails.TotalAmount;
            return new PriceRecalculatedModifyRequest(
                request.OrderNumber,
                pricedLines,
                request.OriginalOrderDetails,
                newTotalPrice,
                priceDifference
            );
        }
    }
}