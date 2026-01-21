using OrderManagement.Domain.Models;
using OrderManagement.Domain.Operations;
using System;
using static OrderManagement.Domain.Models.ModifyOrderRequest;
using static OrderManagement.Domain.Models.OrderModifiedEvent;
namespace OrderManagement.Domain.Workflows
{
    public class ModifyOrderWorkflow
    {
        public IOrderModifiedEvent Execute(
            ModifyOrderCommand command,
            Func<OrderNumber, (bool exists, OrderDetails? details)> checkOrderExists,
            Func<ProductCode, (bool exists, string productName, decimal price)> checkProductCatalog,
            Func<ProductCode, Quantity, bool> checkInventory)
        {
            // Create unvalidated modify request from command
            UnvalidatedModifyRequest unvalidatedRequest = new(
                command.OrderNumber,
                command.NewOrderLines
            );
            // Execute the workflow operations pipeline
            IModifyOrderRequest request = unvalidatedRequest;
            // 1. Validate input data (order number format, product codes, quantities)
            request = new ValidateModifyRequestOperation().Transform(request);
            // 2. Verify order exists and can be modified (business rules: status & time window)
            request = new VerifyOrderCanBeModifiedOperation(checkOrderExists).Transform(request);
            // 3. Verify new products exist in catalog and are in stock
            request = new VerifyNewProductsAndStockOperation(checkProductCatalog, checkInventory).Transform(request);
            // 4. Recalculate price and determine price difference
            request = new RecalculatePriceOperation().Transform(request);
            // 5. Process the modification (finalize)
            request = new ProcessModificationOperation().Transform(request);
            // Convert final state to event
            return request.ToEvent();
        }
    }
}