using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static OrderManagement.Domain.Models.ModifyOrderRequest;
namespace OrderManagement.Domain.Models
{
    public static class OrderModifiedEvent
    {
        public interface IOrderModifiedEvent { }
        public record OrderModifiedSuccessEvent : IOrderModifiedEvent
        {
            public string OrderNumber { get; }
            public decimal NewTotalPrice { get; }
            public decimal PriceDifference { get; }
            public DateTime ModifiedDate { get; }
            public string Summary { get; }
            internal OrderModifiedSuccessEvent(
                string orderNumber,
                decimal newTotalPrice,
                decimal priceDifference,
                DateTime modifiedDate,
                string summary)
            {
                OrderNumber = orderNumber;
                NewTotalPrice = newTotalPrice;
                PriceDifference = priceDifference;
                ModifiedDate = modifiedDate;
                Summary = summary;
            }
        }
        public record OrderModifiedFailedEvent : IOrderModifiedEvent
        {
            public IEnumerable<string> Reasons { get; }
            internal OrderModifiedFailedEvent(string reason)
            {
                Reasons = new[] { reason };
            }
            internal OrderModifiedFailedEvent(IEnumerable<string> reasons)
            {
                Reasons = reasons;
            }
        }
        public static IOrderModifiedEvent ToEvent(this IModifyOrderRequest request) =>
            request switch
            {
                UnvalidatedModifyRequest _ => new OrderModifiedFailedEvent("Unexpected unvalidated state"),
                ValidatedModifyRequest _ => new OrderModifiedFailedEvent("Unexpected validated state"),
                OrderVerifiedModifyRequest _ => new OrderModifiedFailedEvent("Unexpected order verified state"),
                ProductsVerifiedModifyRequest _ => new OrderModifiedFailedEvent("Unexpected products verified state"),
                PriceRecalculatedModifyRequest _ => new OrderModifiedFailedEvent("Unexpected price recalculated state"),
                InvalidModifyRequest invalid => new OrderModifiedFailedEvent(invalid.ValidationErrors),
                ModifiedOrder modified => GenerateSuccessEvent(modified),
                _ => throw new NotImplementedException()
            };
        private static OrderModifiedSuccessEvent GenerateSuccessEvent(ModifiedOrder modified)
        {
            StringBuilder summary = new();
            summary.AppendLine($"Order {modified.OrderNumber} has been successfully modified.");
            summary.AppendLine();
            summary.AppendLine("Modified Order Items:");
            foreach (var line in modified.NewOrderLines)
            {
                summary.AppendLine($"  - {line.ProductName} ({line.ProductCode}) x {line.Quantity} @ {line.Price:C} = {line.LineTotal:C}");
            }
            summary.AppendLine();
            summary.AppendLine($"New Order Total: {modified.NewTotalPrice:C}");
            if (modified.PriceDifference > 0)
            {
                summary.AppendLine($"Additional Charge: {modified.PriceDifference:C}");
                summary.AppendLine("The additional amount will be charged to your payment method.");
            }
            else if (modified.PriceDifference < 0)
            {
                summary.AppendLine($"Refund Amount: {Math.Abs(modified.PriceDifference):C}");
                summary.AppendLine("The refund will be processed within 3-5 business days.");
            }
            else
            {
                summary.AppendLine("Price Difference: None (same total as original order)");
            }
            summary.AppendLine();
            summary.AppendLine($"Modified on: {modified.ModifiedDate:g}");
            return new OrderModifiedSuccessEvent(
                modified.OrderNumber.Value,
                modified.NewTotalPrice,
                modified.PriceDifference,
                modified.ModifiedDate,
                summary.ToString()
            );
        }
    }
}