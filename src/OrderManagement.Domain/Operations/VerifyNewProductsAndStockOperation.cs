using OrderManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static OrderManagement.Domain.Models.ModifyOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class VerifyNewProductsAndStockOperation : ModifyOrderOperation
    {
        private readonly Func<ProductCode, (bool exists, string productName, decimal price)> checkProductCatalog;
        private readonly Func<ProductCode, Quantity, bool> checkInventory;
        public VerifyNewProductsAndStockOperation(
            Func<ProductCode, (bool, string, decimal)> checkProductCatalog,
            Func<ProductCode, Quantity, bool> checkInventory)
        {
            this.checkProductCatalog = checkProductCatalog;
            this.checkInventory = checkInventory;
        }
        protected override IModifyOrderRequest OnOrderVerified(OrderVerifiedModifyRequest request)
        {
            List<string> errors = new();
            List<ProductVerifiedOrderLine> verifiedLines = new();
            // Check each product
            foreach (var line in request.NewOrderLines)
            {
                // Check if product exists in catalog
                var (exists, productName, price) = checkProductCatalog(line.ProductCode);
                if (!exists)
                {
                    errors.Add($"Product not found: {line.ProductCode}");
                    continue;
                }
                // Check if sufficient stock is available
                bool inStock = checkInventory(line.ProductCode, line.Quantity);
                if (!inStock)
                {
                    errors.Add($"Insufficient stock for product {line.ProductCode} ({productName}). Requested: {line.Quantity}");
                    continue;
                }
                // Product verified
                verifiedLines.Add(new ProductVerifiedOrderLine(line.ProductCode, productName, price, line.Quantity));
            }
            // If any errors, return invalid
            if (errors.Any())
            {
                return new InvalidModifyRequest(request.OrderNumber.Value, errors);
            }
            return new ProductsVerifiedModifyRequest(request.OrderNumber, verifiedLines, request.OriginalOrderDetails);
        }
    }
}