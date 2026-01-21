using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static OrderManagement.Domain.Models.ReturnOrderRequest;
namespace OrderManagement.Domain.Models
{
    public static class OrderReturnedEvent
    {
        public interface IOrderReturnedEvent { }
        public record OrderReturnedSuccessEvent : IOrderReturnedEvent
        {
            public string OrderNumber { get; }
            public string ReturnNumber { get; }
            public decimal RefundAmount { get; }
            public decimal ShippingFee { get; }
            public ReturnReasonType ReasonType { get; }
            public DateTime ProcessedDate { get; }
            public string Summary { get; }
            internal OrderReturnedSuccessEvent(
                string orderNumber,
                string returnNumber,
                decimal refundAmount,
                decimal shippingFee,
                ReturnReasonType reasonType,
                DateTime processedDate,
                string summary)
            {
                OrderNumber = orderNumber;
                ReturnNumber = returnNumber;
                RefundAmount = refundAmount;
                ShippingFee = shippingFee;
                ReasonType = reasonType;
                ProcessedDate = processedDate;
                Summary = summary;
            }
        }
        public record OrderReturnedFailedEvent : IOrderReturnedEvent
        {
            public IEnumerable<string> Reasons { get; }
            internal OrderReturnedFailedEvent(string reason)
            {
                Reasons = new[] { reason };
            }
            internal OrderReturnedFailedEvent(IEnumerable<string> reasons)
            {
                Reasons = reasons;
            }
        }
        public static IOrderReturnedEvent ToEvent(this IReturnOrderRequest request) =>
            request switch
            {
                UnvalidatedReturnRequest _ => new OrderReturnedFailedEvent("Unexpected unvalidated state"),
                ValidatedReturnRequest _ => new OrderReturnedFailedEvent("Unexpected validated state"),
                OrderVerifiedReturnRequest _ => new OrderReturnedFailedEvent("Unexpected order verified state"),
                ReturnApprovedRequest _ => new OrderReturnedFailedEvent("Unexpected return approved state"),
                InvalidReturnRequest invalid => new OrderReturnedFailedEvent(invalid.ValidationErrors),
                ProcessedReturn processed => GenerateSuccessEvent(processed),
                _ => throw new NotImplementedException()
            };
        private static OrderReturnedSuccessEvent GenerateSuccessEvent(ProcessedReturn processed)
        {
            StringBuilder summary = new();
            summary.AppendLine($"Return request has been successfully processed.");
            summary.AppendLine();
            summary.AppendLine($"Order Number: {processed.OrderNumber}");
            summary.AppendLine($"Return Number: {processed.ReturnNumber}");
            summary.AppendLine();
            summary.AppendLine("Return Details:");
            summary.AppendLine($"  Return Reason: {processed.ReturnReason}");
            summary.AppendLine($"  Reason Category: {GetReasonTypeDescription(processed.ReturnReason.Type)}");
            summary.AppendLine();
            summary.AppendLine("Returned Items:");
            foreach (var item in processed.ReturnItems)
            {
                summary.AppendLine($"  - Product: {item.ProductCode}, Quantity: {item.Quantity}");
            }
            summary.AppendLine();
            if (processed.ShippingFee > 0)
            {
                summary.AppendLine($"Shipping Fee: {processed.ShippingFee:C}");
            }
            else
            {
                summary.AppendLine("Shipping Fee: None (company responsibility)");
            }
            summary.AppendLine($"Refund Amount: {processed.RefundAmount:C}");
            summary.AppendLine();
            summary.AppendLine("The refund will be processed within 5-7 business days.");
            summary.AppendLine("You will receive return instructions via email.");
            return new OrderReturnedSuccessEvent(
                processed.OrderNumber.Value,
                processed.ReturnNumber,
                processed.RefundAmount,
                processed.ShippingFee,
                processed.ReturnReason.Type,
                processed.ProcessedDate,
                summary.ToString()
            );
        }
        private static string GetReasonTypeDescription(ReturnReasonType type) => type switch
        {
            ReturnReasonType.Defective => "Defective Product",
            ReturnReasonType.WrongItem => "Wrong Item Shipped",
            ReturnReasonType.NotAsDescribed => "Not As Described",
            ReturnReasonType.Changed_Mind => "Customer Changed Mind",
            _ => "Other"
        };
    }
}