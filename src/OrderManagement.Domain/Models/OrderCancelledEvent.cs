using System;
using static OrderManagement.Domain.Models.CancelOrderRequest;
namespace OrderManagement.Domain.Models
{
    public static class OrderCancelledEvent
    {
        public interface IOrderCancelledEvent { }
        public record OrderCancelledSuccessEvent : IOrderCancelledEvent
        {
            public string OrderNumber { get; }
            public decimal RefundAmount { get; }
            public DateTime CancelledDate { get; }
            public string Summary { get; }
            internal OrderCancelledSuccessEvent(string orderNumber, decimal refundAmount, DateTime cancelledDate, string summary)
            {
                OrderNumber = orderNumber;
                RefundAmount = refundAmount;
                CancelledDate = cancelledDate;
                Summary = summary;
            }
        }
        public record OrderCancelledFailedEvent : IOrderCancelledEvent
        {
            public string Reason { get; }
            internal OrderCancelledFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
        public static IOrderCancelledEvent ToEvent(this ICancelOrderRequest request) =>
            request switch
            {
                UnvalidatedCancelRequest _ => new OrderCancelledFailedEvent("Unexpected unvalidated state"),
                ValidatedCancelRequest _ => new OrderCancelledFailedEvent("Unexpected validated state"),
                OrderVerifiedCancelRequest _ => new OrderCancelledFailedEvent("Unexpected order verified state"),
                RefundCalculatedCancelRequest _ => new OrderCancelledFailedEvent("Unexpected refund calculated state"),
                InvalidCancelRequest invalid => new OrderCancelledFailedEvent(invalid.ValidationError),
                CancelledOrder cancelled => GenerateSuccessEvent(cancelled),
                _ => throw new NotImplementedException()
            };
        private static OrderCancelledSuccessEvent GenerateSuccessEvent(CancelledOrder cancelled)
        {
            decimal refundPercentage = cancelled.OrderDetails.TotalAmount > 0 
                ? (cancelled.RefundAmount / cancelled.OrderDetails.TotalAmount) * 100 
                : 0;
            string summary = $@"Order {cancelled.OrderNumber} has been successfully cancelled.
Cancellation Details:
- Order Number: {cancelled.OrderNumber}
- Original Order Total: {cancelled.OrderDetails.TotalAmount:C}
- Refund Amount: {cancelled.RefundAmount:C} ({refundPercentage:F0}% of total)
- Cancellation Reason: {cancelled.Reason}
- Cancelled Date: {cancelled.CancelledDate:g}
The refund will be processed within 3-5 business days.";
            return new OrderCancelledSuccessEvent(
                cancelled.OrderNumber.Value,
                cancelled.RefundAmount,
                cancelled.CancelledDate,
                summary
            );
        }
    }
}