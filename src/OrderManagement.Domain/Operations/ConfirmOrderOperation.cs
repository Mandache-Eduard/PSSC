using OrderManagement.Domain.Models;
using System;
using static OrderManagement.Domain.Models.Order;
namespace OrderManagement.Domain.Operations
{
    internal sealed class ConfirmOrderOperation : OrderOperation
    {
        protected override IOrder OnPriced(PricedOrder order)
        {
            // Generate a unique order number
            string orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            return new ConfirmedOrder(order.OrderLines, order.ShippingAddress, order.TotalPrice, orderNumber);
        }
    }
}