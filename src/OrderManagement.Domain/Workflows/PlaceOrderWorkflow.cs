using OrderManagement.Domain.Models;
using OrderManagement.Domain.Operations;
using System;
using static OrderManagement.Domain.Models.Order;
using static OrderManagement.Domain.Models.OrderPlacedEvent;
namespace OrderManagement.Domain.Workflows
{
    public class PlaceOrderWorkflow
    {
        public IOrderPlacedEvent Execute(
            PlaceOrderCommand command,
            Func<ProductCode, (bool exists, string productName, decimal price)> checkProductCatalog,
            Func<ProductCode, Quantity, bool> checkInventory)
        {
            // Create unvalidated order from command
            UnvalidatedOrder unvalidatedOrder = new(
                command.OrderLines,
                command.Street,
                command.City,
                command.PostalCode,
                command.Country
            );
            // Execute the workflow operations pipeline
            IOrder order = unvalidatedOrder;
            // 1. Validate input data and convert to DDD types
            order = new ValidateOrderOperation().Transform(order);
            // 2. Check product existence
            order = new CheckProductExistsOperation(checkProductCatalog).Transform(order);
            // 3. Check stock availability
            order = new CheckStockOperation(checkInventory).Transform(order);
            // 4. Calculate price (address already validated in step 1)
            order = new CalculatePriceOperation().Transform(order);
            // 5. Confirm order
            order = new ConfirmOrderOperation().Transform(order);
            // Convert final state to event
            return order.ToEvent();
        }
    }
}