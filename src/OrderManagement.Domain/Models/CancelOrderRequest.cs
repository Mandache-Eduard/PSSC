using System;
namespace OrderManagement.Domain.Models
{
    public static class CancelOrderRequest
    {
        public interface ICancelOrderRequest { }
        // State 1: Unvalidated - raw input from user
        public record UnvalidatedCancelRequest : ICancelOrderRequest
        {
            public UnvalidatedCancelRequest(string orderNumber, string reason)
            {
                OrderNumber = orderNumber;
                Reason = reason;
            }
            public string OrderNumber { get; }
            public string Reason { get; }
        }
        // State 2: Invalid - validation failed
        public record InvalidCancelRequest : ICancelOrderRequest
        {
            internal InvalidCancelRequest(string orderNumber, string reason, string validationError)
            {
                OrderNumber = orderNumber;
                Reason = reason;
                ValidationError = validationError;
            }
            public string OrderNumber { get; }
            public string Reason { get; }
            public string ValidationError { get; }
        }
        // State 3: Validated - input validated, typed
        public record ValidatedCancelRequest : ICancelOrderRequest
        {
            internal ValidatedCancelRequest(OrderNumber orderNumber, CancellationReason reason)
            {
                OrderNumber = orderNumber;
                Reason = reason;
            }
            public OrderNumber OrderNumber { get; }
            public CancellationReason Reason { get; }
        }
        // State 4: OrderVerified - order exists and can be cancelled
        public record OrderVerifiedCancelRequest : ICancelOrderRequest
        {
            internal OrderVerifiedCancelRequest(OrderNumber orderNumber, CancellationReason reason, OrderDetails orderDetails)
            {
                OrderNumber = orderNumber;
                Reason = reason;
                OrderDetails = orderDetails;
            }
            public OrderNumber OrderNumber { get; }
            public CancellationReason Reason { get; }
            public OrderDetails OrderDetails { get; }
        }
        // State 5: RefundCalculated - refund amount determined
        public record RefundCalculatedCancelRequest : ICancelOrderRequest
        {
            internal RefundCalculatedCancelRequest(OrderNumber orderNumber, CancellationReason reason, OrderDetails orderDetails, decimal refundAmount)
            {
                OrderNumber = orderNumber;
                Reason = reason;
                OrderDetails = orderDetails;
                RefundAmount = refundAmount;
            }
            public OrderNumber OrderNumber { get; }
            public CancellationReason Reason { get; }
            public OrderDetails OrderDetails { get; }
            public decimal RefundAmount { get; }
        }
        // State 6: Cancelled - cancellation completed
        public record CancelledOrder : ICancelOrderRequest
        {
            internal CancelledOrder(OrderNumber orderNumber, CancellationReason reason, OrderDetails orderDetails, decimal refundAmount, DateTime cancelledDate)
            {
                OrderNumber = orderNumber;
                Reason = reason;
                OrderDetails = orderDetails;
                RefundAmount = refundAmount;
                CancelledDate = cancelledDate;
            }
            public OrderNumber OrderNumber { get; }
            public CancellationReason Reason { get; }
            public OrderDetails OrderDetails { get; }
            public decimal RefundAmount { get; }
            public DateTime CancelledDate { get; }
        }
    }
    // Supporting value objects
    public record OrderNumber
    {
        public string Value { get; }
        private OrderNumber(string value)
        {
            Value = value;
        }
        public static bool TryParse(string? orderNumberString, out OrderNumber? orderNumber)
        {
            orderNumber = null;
            if (string.IsNullOrWhiteSpace(orderNumberString))
                return false;
            // Format: ORD-YYYYMMDD-XXXXXXXX
            if (!orderNumberString.StartsWith("ORD-") || orderNumberString.Length < 20)
                return false;
            orderNumber = new OrderNumber(orderNumberString);
            return true;
        }
        public override string ToString() => Value;
    }
    public record CancellationReason
    {
        public string Value { get; }
        private CancellationReason(string value)
        {
            Value = value;
        }
        public static bool TryParse(string? reasonString, out CancellationReason? reason)
        {
            reason = null;
            if (string.IsNullOrWhiteSpace(reasonString))
                return false;
            if (reasonString.Length < 10)
                return false; // Reason must be at least 10 characters
            reason = new CancellationReason(reasonString);
            return true;
        }
        public override string ToString() => Value;
    }
    public record OrderDetails
    {
        public decimal TotalAmount { get; }
        public DateTime OrderDate { get; }
        public string Status { get; }
        public OrderDetails(decimal totalAmount, DateTime orderDate, string status)
        {
            TotalAmount = totalAmount;
            OrderDate = orderDate;
            Status = status;
        }
    }
}