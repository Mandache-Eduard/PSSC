using OrderManagement.Domain.Models;
using OrderManagement.Domain.Operations;
using OrderManagement.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using static OrderManagement.Domain.Models.Order;
using static OrderManagement.Domain.Models.OrderPlacedEvent;

namespace OrderManagement.Domain.Workflows
{
    public class PlaceOrderWorkflow
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IOrdersRepository _ordersRepository;

        public PlaceOrderWorkflow(IProductsRepository productsRepository, IOrdersRepository ordersRepository)
        {
            _productsRepository = productsRepository;
            _ordersRepository = ordersRepository;
        }

        public async Task<IOrderPlacedEvent> ExecuteAsync(PlaceOrderCommand command)
        {
            try
            {
                // STEP 1: LOAD STATE FROM DATABASE
                // Get product codes from command
                var productCodes = command.OrderLines
                    .Select(line => line.ProductCode)
                    .Where(code => ProductCode.TryParse(code, out _))
                    .Select(code => { ProductCode.TryParse(code, out var pc); return pc!; })
                    .ToList();

                // Load products from database
                var productsData = await _productsRepository.GetProductsByCodesAsync(productCodes);
                
                // Create check functions using loaded data
                Func<ProductCode, (bool exists, string productName, decimal price)> checkProductCatalog = 
                    productCode => _productsRepository.GetProductByCodeAsync(productCode).Result;
                
                Func<ProductCode, Quantity, bool> checkInventory = 
                    (productCode, quantity) => _productsRepository.CheckStockAsync(productCode, quantity).Result;

                // STEP 2: EXECUTE PURE BUSINESS LOGIC (no database access!)
                IOrder order = ExecuteBusinessLogic(command, checkProductCatalog, checkInventory);

                // STEP 3: SAVE RESULTS TO DATABASE
                if (order is ConfirmedOrder confirmedOrder)
                {
                    await _ordersRepository.SaveCompleteOrderAsync(confirmedOrder);
                }

                // Convert final state to event
                return order.ToEvent();
            }
            catch (Exception ex)
            {
                return new OrderPlacedFailedEvent(new[] { $"Unexpected error: {ex.Message}" });
            }
        }

        private static IOrder ExecuteBusinessLogic(
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
            
            // Execute the workflow operations pipeline (pure functions)
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
            
            return order;
        }
    }
}

