using OrderManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static OrderManagement.Domain.Models.Order;
namespace OrderManagement.Domain.Operations
{
    internal sealed class CheckProductExistsOperation : OrderOperation
    {
        private readonly Func<ProductCode, (bool exists, string productName, decimal price)> checkProductCatalog;
        public CheckProductExistsOperation(Func<ProductCode, (bool, string, decimal)> checkProductCatalog)
        {
            this.checkProductCatalog = checkProductCatalog;
        }
        protected override IOrder OnValidated(ValidatedOrder order)
        {
            List<string> errors = new();
            List<ProductVerifiedOrderLine> verifiedLines = new();
            foreach (var line in order.OrderLines)
            {
                var (exists, productName, price) = checkProductCatalog(line.ProductCode);
                if (!exists)
                {
                    errors.Add($"Product not found: {line.ProductCode}");
                }
                else
                {
                    verifiedLines.Add(new ProductVerifiedOrderLine(line.ProductCode, productName, price, line.Quantity));
                }
            }
            if (errors.Any())
            {
                // Convert back to unvalidated for error reporting
                var unvalidatedLines = order.OrderLines.Select(l => 
                    new UnvalidatedOrderLine(l.ProductCode.Value, l.Quantity.Value.ToString())).ToList();
                return new InvalidOrder(unvalidatedLines, errors);
            }
            return new ProductVerifiedOrder(verifiedLines, order.ShippingAddress);
        }
    }
}