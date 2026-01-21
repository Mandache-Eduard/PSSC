using OrderManagement.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using static OrderManagement.Domain.Models.Order;
namespace OrderManagement.Domain.Operations
{
    internal sealed class ValidateOrderOperation : OrderOperation
    {
        protected override IOrder OnUnvalidated(UnvalidatedOrder order)
        {
            List<string> validationErrors = new();
            List<ValidatedOrderLine> validatedLines = new();
            // Validate order lines
            foreach (var line in order.OrderLines)
            {
                var validLine = ValidateOrderLine(line, validationErrors);
                if (validLine != null)
                {
                    validatedLines.Add(validLine);
                }
            }
            // Validate address
            if (!Address.TryParse(order.Street, order.City, order.PostalCode, order.Country, out Address? address))
            {
                validationErrors.Add("Invalid shipping address: all fields must be provided");
            }
            // Return result
            if (validationErrors.Any())
            {
                return new InvalidOrder(order.OrderLines, validationErrors);
            }
            else
            {
                return new ValidatedOrder(validatedLines, address!);
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