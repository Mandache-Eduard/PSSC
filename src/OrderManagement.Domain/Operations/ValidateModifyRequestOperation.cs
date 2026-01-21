using OrderManagement.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using static OrderManagement.Domain.Models.ModifyOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class ValidateModifyRequestOperation : ModifyOrderOperation
    {
        protected override IModifyOrderRequest OnUnvalidated(UnvalidatedModifyRequest request)
        {
            List<string> validationErrors = new();
            List<ValidatedOrderLine> validatedLines = new();
            // Validate order number
            if (!OrderNumber.TryParse(request.OrderNumber, out OrderNumber? orderNumber))
            {
                validationErrors.Add($"Invalid order number format: {request.OrderNumber}. Expected format: ORD-YYYYMMDD-XXXXXXXX");
            }
            // Validate that at least one product is provided
            if (request.NewOrderLines == null || !request.NewOrderLines.Any())
            {
                validationErrors.Add("At least one product must be specified for order modification");
            }
            else
            {
                // Validate each order line
                foreach (var line in request.NewOrderLines)
                {
                    var validLine = ValidateOrderLine(line, validationErrors);
                    if (validLine != null)
                    {
                        validatedLines.Add(validLine);
                    }
                }
            }
            // Return result
            if (validationErrors.Any())
            {
                return new InvalidModifyRequest(request.OrderNumber, validationErrors);
            }
            else
            {
                return new ValidatedModifyRequest(orderNumber!, validatedLines);
            }
        }
        private static ValidatedOrderLine? ValidateOrderLine(UnvalidatedOrderLine line, List<string> errors)
        {
            List<string> lineErrors = new();
            if (!ProductCode.TryParse(line.ProductCode, out ProductCode? productCode))
            {
                lineErrors.Add($"Invalid product code: {line.ProductCode}");
            }
            if (!Quantity.TryParse(line.Quantity, out Quantity? quantity))
            {
                lineErrors.Add($"Invalid quantity for product {line.ProductCode}: {line.Quantity}");
            }
            if (lineErrors.Any())
            {
                errors.AddRange(lineErrors);
                return null;
            }
            return new ValidatedOrderLine(productCode!, quantity!);
        }
    }
}