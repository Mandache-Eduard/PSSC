using OrderManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static OrderManagement.Domain.Models.Order;
namespace OrderManagement.Domain.Operations
{
    internal sealed class CheckStockOperation : OrderOperation
    {
        private readonly Func<ProductCode, Quantity, bool> checkInventory;
        public CheckStockOperation(Func<ProductCode, Quantity, bool> checkInventory)
        {
            this.checkInventory = checkInventory;
        }
        protected override IOrder OnProductVerified(ProductVerifiedOrder order)
        {
            List<string> errors = new();
            foreach (var line in order.OrderLines)
            {
                bool inStock = checkInventory(line.ProductCode, line.Quantity);
                if (!inStock)
                {
                    errors.Add($"Insufficient stock for product {line.ProductCode} ({line.ProductName}). Requested: {line.Quantity}");
                }
            }
            if (errors.Any())
            {
                var unvalidatedLines = order.OrderLines.Select(l => 
                    new UnvalidatedOrderLine(l.ProductCode.Value, l.Quantity.Value.ToString())).ToList();
                return new InvalidOrder(unvalidatedLines, errors);
            }
            // Pass through - stock is OK
            return order;
        }
    }
}