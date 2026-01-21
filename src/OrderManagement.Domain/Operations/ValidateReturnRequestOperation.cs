using OrderManagement.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using static OrderManagement.Domain.Models.ReturnOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class ValidateReturnRequestOperation : ReturnOrderOperation
    {
        protected override IReturnOrderRequest OnUnvalidated(UnvalidatedReturnRequest request)
        {
            List<string> validationErrors = new();
            List<ValidatedReturnItem> validatedItems = new();
            // Validate order number
            if (!OrderNumber.TryParse(request.OrderNumber, out OrderNumber? orderNumber))
            {
                validationErrors.Add($"Invalid order number format: {request.OrderNumber}. Expected format: ORD-YYYYMMDD-XXXXXXXX");
            }
            // Validate return reason
            if (!ReturnReason.TryParse(request.ReturnReason, out ReturnReason? returnReason))
            {
                validationErrors.Add("Return reason must be at least 10 characters long");
            }
            // Validate that at least one item is being returned
            if (request.ReturnItems == null || !request.ReturnItems.Any())
            {
                validationErrors.Add("At least one item must be specified for return");
            }
            else
            {
                // Validate each return item
                foreach (var item in request.ReturnItems)
                {
                    var validItem = ValidateReturnItem(item, validationErrors);
                    if (validItem != null)
                    {
                        validatedItems.Add(validItem);
                    }
                }
            }
            // Return result
            if (validationErrors.Any())
            {
                return new InvalidReturnRequest(request.OrderNumber, validationErrors);
            }
            else
            {
                return new ValidatedReturnRequest(orderNumber!, returnReason!, validatedItems);
            }
        }
        private static ValidatedReturnItem? ValidateReturnItem(UnvalidatedReturnItem item, List<string> errors)
        {
            List<string> itemErrors = new();
            if (!ProductCode.TryParse(item.ProductCode, out ProductCode? productCode))
            {
                itemErrors.Add($"Invalid product code: {item.ProductCode}");
            }
            if (!Quantity.TryParse(item.Quantity, out Quantity? quantity))
            {
                itemErrors.Add($"Invalid quantity for product {item.ProductCode}: {item.Quantity}");
            }
            if (itemErrors.Any())
            {
                errors.AddRange(itemErrors);
                return null;
            }
            return new ValidatedReturnItem(productCode!, quantity!);
        }
    }
}