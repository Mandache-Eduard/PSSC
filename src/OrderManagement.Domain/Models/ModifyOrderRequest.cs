using System;
using System.Collections.Generic;
namespace OrderManagement.Domain.Models
{
    public static class ModifyOrderRequest
    {
        public interface IModifyOrderRequest { }
        // State 1: Unvalidated - raw input from user
        public record UnvalidatedModifyRequest : IModifyOrderRequest
        {
            public UnvalidatedModifyRequest(string orderNumber, IReadOnlyCollection<UnvalidatedOrderLine> newOrderLines)
            {
                OrderNumber = orderNumber;
                NewOrderLines = newOrderLines;
            }
            public string OrderNumber { get; }
            public IReadOnlyCollection<UnvalidatedOrderLine> NewOrderLines { get; }
        }
        // State 2: Invalid - validation failed
        public record InvalidModifyRequest : IModifyOrderRequest
        {
            internal InvalidModifyRequest(string orderNumber, IEnumerable<string> validationErrors)
            {
                OrderNumber = orderNumber;
                ValidationErrors = validationErrors;
            }
            public string OrderNumber { get; }
            public IEnumerable<string> ValidationErrors { get; }
        }
        // State 3: Validated - input validated, typed
        public record ValidatedModifyRequest : IModifyOrderRequest
        {
            internal ValidatedModifyRequest(OrderNumber orderNumber, IReadOnlyCollection<ValidatedOrderLine> newOrderLines)
            {
                OrderNumber = orderNumber;
                NewOrderLines = newOrderLines;
            }
            public OrderNumber OrderNumber { get; }
            public IReadOnlyCollection<ValidatedOrderLine> NewOrderLines { get; }
        }
        // State 4: OrderVerified - order exists and can be modified
        public record OrderVerifiedModifyRequest : IModifyOrderRequest
        {
            internal OrderVerifiedModifyRequest(
                OrderNumber orderNumber, 
                IReadOnlyCollection<ValidatedOrderLine> newOrderLines,
                OrderDetails originalOrderDetails)
            {
                OrderNumber = orderNumber;
                NewOrderLines = newOrderLines;
                OriginalOrderDetails = originalOrderDetails;
            }
            public OrderNumber OrderNumber { get; }
            public IReadOnlyCollection<ValidatedOrderLine> NewOrderLines { get; }
            public OrderDetails OriginalOrderDetails { get; }
        }
        // State 5: ProductsVerified - products exist and in stock
        public record ProductsVerifiedModifyRequest : IModifyOrderRequest
        {
            internal ProductsVerifiedModifyRequest(
                OrderNumber orderNumber,
                IReadOnlyCollection<ProductVerifiedOrderLine> newOrderLines,
                OrderDetails originalOrderDetails)
            {
                OrderNumber = orderNumber;
                NewOrderLines = newOrderLines;
                OriginalOrderDetails = originalOrderDetails;
            }
            public OrderNumber OrderNumber { get; }
            public IReadOnlyCollection<ProductVerifiedOrderLine> NewOrderLines { get; }
            public OrderDetails OriginalOrderDetails { get; }
        }
        // State 6: PriceRecalculated - new total calculated
        public record PriceRecalculatedModifyRequest : IModifyOrderRequest
        {
            internal PriceRecalculatedModifyRequest(
                OrderNumber orderNumber,
                IReadOnlyCollection<PricedOrderLine> newOrderLines,
                OrderDetails originalOrderDetails,
                decimal newTotalPrice,
                decimal priceDifference)
            {
                OrderNumber = orderNumber;
                NewOrderLines = newOrderLines;
                OriginalOrderDetails = originalOrderDetails;
                NewTotalPrice = newTotalPrice;
                PriceDifference = priceDifference;
            }
            public OrderNumber OrderNumber { get; }
            public IReadOnlyCollection<PricedOrderLine> NewOrderLines { get; }
            public OrderDetails OriginalOrderDetails { get; }
            public decimal NewTotalPrice { get; }
            public decimal PriceDifference { get; }  // Positive = customer pays more, Negative = refund
        }
        // State 7: Modified - modification completed
        public record ModifiedOrder : IModifyOrderRequest
        {
            internal ModifiedOrder(
                OrderNumber orderNumber,
                IReadOnlyCollection<PricedOrderLine> newOrderLines,
                decimal newTotalPrice,
                decimal priceDifference,
                DateTime modifiedDate)
            {
                OrderNumber = orderNumber;
                NewOrderLines = newOrderLines;
                NewTotalPrice = newTotalPrice;
                PriceDifference = priceDifference;
                ModifiedDate = modifiedDate;
            }
            public OrderNumber OrderNumber { get; }
            public IReadOnlyCollection<PricedOrderLine> NewOrderLines { get; }
            public decimal NewTotalPrice { get; }
            public decimal PriceDifference { get; }
            public DateTime ModifiedDate { get; }
        }
    }
}