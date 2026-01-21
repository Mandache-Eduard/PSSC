using System;
using System.Collections.Generic;
namespace OrderManagement.Domain.Models
{
    public static class ReturnOrderRequest
    {
        public interface IReturnOrderRequest { }
        // State 1: Unvalidated - raw input from user
        public record UnvalidatedReturnRequest : IReturnOrderRequest
        {
            public UnvalidatedReturnRequest(string orderNumber, string returnReason, IReadOnlyCollection<UnvalidatedReturnItem> returnItems)
            {
                OrderNumber = orderNumber;
                ReturnReason = returnReason;
                ReturnItems = returnItems;
            }
            public string OrderNumber { get; }
            public string ReturnReason { get; }
            public IReadOnlyCollection<UnvalidatedReturnItem> ReturnItems { get; }
        }
        // State 2: Invalid - validation failed
        public record InvalidReturnRequest : IReturnOrderRequest
        {
            internal InvalidReturnRequest(string orderNumber, IEnumerable<string> validationErrors)
            {
                OrderNumber = orderNumber;
                ValidationErrors = validationErrors;
            }
            public string OrderNumber { get; }
            public IEnumerable<string> ValidationErrors { get; }
        }
        // State 3: Validated - input validated, typed
        public record ValidatedReturnRequest : IReturnOrderRequest
        {
            internal ValidatedReturnRequest(OrderNumber orderNumber, ReturnReason returnReason, IReadOnlyCollection<ValidatedReturnItem> returnItems)
            {
                OrderNumber = orderNumber;
                ReturnReason = returnReason;
                ReturnItems = returnItems;
            }
            public OrderNumber OrderNumber { get; }
            public ReturnReason ReturnReason { get; }
            public IReadOnlyCollection<ValidatedReturnItem> ReturnItems { get; }
        }
        // State 4: OrderVerified - order exists and can be returned
        public record OrderVerifiedReturnRequest : IReturnOrderRequest
        {
            internal OrderVerifiedReturnRequest(
                OrderNumber orderNumber,
                ReturnReason returnReason,
                IReadOnlyCollection<ValidatedReturnItem> returnItems,
                OrderDetails originalOrderDetails)
            {
                OrderNumber = orderNumber;
                ReturnReason = returnReason;
                ReturnItems = returnItems;
                OriginalOrderDetails = originalOrderDetails;
            }
            public OrderNumber OrderNumber { get; }
            public ReturnReason ReturnReason { get; }
            public IReadOnlyCollection<ValidatedReturnItem> ReturnItems { get; }
            public OrderDetails OriginalOrderDetails { get; }
        }
        // State 5: ReturnApproved - return approved, refund calculated
        public record ReturnApprovedRequest : IReturnOrderRequest
        {
            internal ReturnApprovedRequest(
                OrderNumber orderNumber,
                ReturnReason returnReason,
                IReadOnlyCollection<ValidatedReturnItem> returnItems,
                OrderDetails originalOrderDetails,
                decimal refundAmount,
                decimal shippingFee)
            {
                OrderNumber = orderNumber;
                ReturnReason = returnReason;
                ReturnItems = returnItems;
                OriginalOrderDetails = originalOrderDetails;
                RefundAmount = refundAmount;
                ShippingFee = shippingFee;
            }
            public OrderNumber OrderNumber { get; }
            public ReturnReason ReturnReason { get; }
            public IReadOnlyCollection<ValidatedReturnItem> ReturnItems { get; }
            public OrderDetails OriginalOrderDetails { get; }
            public decimal RefundAmount { get; }
            public decimal ShippingFee { get; }
        }
        // State 6: ReturnProcessed - return completed
        public record ProcessedReturn : IReturnOrderRequest
        {
            internal ProcessedReturn(
                OrderNumber orderNumber,
                ReturnReason returnReason,
                IReadOnlyCollection<ValidatedReturnItem> returnItems,
                decimal refundAmount,
                decimal shippingFee,
                DateTime processedDate,
                string returnNumber)
            {
                OrderNumber = orderNumber;
                ReturnReason = returnReason;
                ReturnItems = returnItems;
                RefundAmount = refundAmount;
                ShippingFee = shippingFee;
                ProcessedDate = processedDate;
                ReturnNumber = returnNumber;
            }
            public OrderNumber OrderNumber { get; }
            public ReturnReason ReturnReason { get; }
            public IReadOnlyCollection<ValidatedReturnItem> ReturnItems { get; }
            public decimal RefundAmount { get; }
            public decimal ShippingFee { get; }
            public DateTime ProcessedDate { get; }
            public string ReturnNumber { get; }
        }
    }
    // Supporting types for returns
    public record UnvalidatedReturnItem(string ProductCode, string Quantity);
    public record ValidatedReturnItem(ProductCode ProductCode, Quantity Quantity);
    public record ReturnReason
    {
        public string Value { get; }
        public ReturnReasonType Type { get; }
        private ReturnReason(string value, ReturnReasonType type)
        {
            Value = value;
            Type = type;
        }
        public static bool TryParse(string? reasonString, out ReturnReason? reason)
        {
            reason = null;
            if (string.IsNullOrWhiteSpace(reasonString))
                return false;
            if (reasonString.Length < 10)
                return false; // Reason must be at least 10 characters
            // Determine return reason type based on keywords
            var type = DetermineReasonType(reasonString);
            reason = new ReturnReason(reasonString, type);
            return true;
        }
        private static ReturnReasonType DetermineReasonType(string reasonString)
        {
            var lowerReason = reasonString.ToLower();
            if (lowerReason.Contains("defect") || lowerReason.Contains("damaged") || lowerReason.Contains("broken"))
                return ReturnReasonType.Defective;
            if (lowerReason.Contains("wrong") || lowerReason.Contains("incorrect") || lowerReason.Contains("mistake"))
                return ReturnReasonType.WrongItem;
            if (lowerReason.Contains("not as described") || lowerReason.Contains("different"))
                return ReturnReasonType.NotAsDescribed;
            return ReturnReasonType.Changed_Mind;
        }
        public override string ToString() => Value;
    }
    public enum ReturnReasonType
    {
        Defective,           // Product defective - No shipping fee, full refund
        WrongItem,           // Wrong item shipped - No shipping fee, full refund
        NotAsDescribed,      // Not as described - No shipping fee, full refund
        Changed_Mind         // Customer changed mind - Customer pays shipping fee
    }
}