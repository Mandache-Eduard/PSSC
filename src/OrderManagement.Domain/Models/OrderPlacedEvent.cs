using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static OrderManagement.Domain.Models.Order;
namespace OrderManagement.Domain.Models
{
    public static class OrderPlacedEvent
    {
        public interface IOrderPlacedEvent { }
        public record OrderPlacedSuccessEvent : IOrderPlacedEvent
        {
            public string OrderNumber { get; }
            public decimal TotalPrice { get; }
            public string Summary { get; }
            public DateTime PlacedDate { get; }
            internal OrderPlacedSuccessEvent(string orderNumber, decimal totalPrice, string summary, DateTime placedDate)
            {
                OrderNumber = orderNumber;
                TotalPrice = totalPrice;
                Summary = summary;
                PlacedDate = placedDate;
            }
        }
        public record OrderPlacedFailedEvent : IOrderPlacedEvent
        {
            public IEnumerable<string> Reasons { get; }
            internal OrderPlacedFailedEvent(string reason)
            {
                Reasons = new[] { reason };
            }
            internal OrderPlacedFailedEvent(IEnumerable<string> reasons)
            {
                Reasons = reasons;
            }
        }
        public static IOrderPlacedEvent ToEvent(this IOrder order) =>
            order switch
            {
                UnvalidatedOrder _ => new OrderPlacedFailedEvent("Unexpected unvalidated state"),
                ValidatedOrder _ => new OrderPlacedFailedEvent("Unexpected validated state"),
                ProductVerifiedOrder _ => new OrderPlacedFailedEvent("Unexpected product verified state"),
                PricedOrder _ => new OrderPlacedFailedEvent("Unexpected priced state"),
                InvalidOrder invalidOrder => new OrderPlacedFailedEvent(invalidOrder.Reasons),
                ConfirmedOrder confirmedOrder => GenerateSuccessEvent(confirmedOrder),
                _ => throw new NotImplementedException()
            };
        private static OrderPlacedSuccessEvent GenerateSuccessEvent(ConfirmedOrder order)
        {
            StringBuilder summary = new();
            summary.AppendLine($"Order Number: {order.OrderNumber}");
            summary.AppendLine($"Shipping Address: {order.ShippingAddress}");
            summary.AppendLine();
            summary.AppendLine("Order Items:");
            foreach (var line in order.OrderLines)
            {
                summary.AppendLine($"  - {line.ProductName} ({line.ProductCode}) x {line.Quantity} @ {line.Price:C} = {line.LineTotal:C}");
            }
            summary.AppendLine();
            summary.AppendLine($"Total: {order.TotalPrice:C}");
            return new OrderPlacedSuccessEvent(
                order.OrderNumber,
                order.TotalPrice,
                summary.ToString(),
                DateTime.Now
            );
        }
    }
}