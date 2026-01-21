using OrderManagement.Domain.Models;
using System.Linq;
using static OrderManagement.Domain.Models.Order;
namespace OrderManagement.Domain.Operations
{
    internal sealed class CalculatePriceOperation : OrderOperation
    {
        protected override IOrder OnProductVerified(ProductVerifiedOrder order)
        {
            // Transform ProductVerifiedOrderLine to PricedOrderLine and calculate totals
            var pricedLines = order.OrderLines
                .Select(line => PricedOrderLine.FromVerifiedLine(line))
                .ToList();
            decimal totalPrice = pricedLines.Sum(line => line.LineTotal);
            return new PricedOrder(pricedLines, order.ShippingAddress, totalPrice);
        }
    }
}