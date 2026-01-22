using OrderManagement.Domain.Models;
using OrderManagement.Domain.Operations;
using OrderManagement.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using static OrderManagement.Domain.Models.ModifyOrderRequest;
using static OrderManagement.Domain.Models.OrderModifiedEvent;

namespace OrderManagement.Domain.Workflows
{
    public class ModifyOrderWorkflow
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IProductsRepository _productsRepository;

        public ModifyOrderWorkflow(IOrdersRepository ordersRepository, IProductsRepository productsRepository)
        {
            _ordersRepository = ordersRepository;
            _productsRepository = productsRepository;
        }

        public async Task<IOrderModifiedEvent> ExecuteAsync(ModifyOrderCommand command)
        {
            try
            {
                // STEP 1: LOAD STATE FROM DATABASE
                // Get product codes from command to load from database
                var productCodes = command.NewOrderLines
                    .Select(line => line.ProductCode)
                    .Where(code => ProductCode.TryParse(code, out _))
                    .Select(code => { ProductCode.TryParse(code, out var pc); return pc!; })
                    .ToList();

                // Load products from database
                var productsData = await _productsRepository.GetProductsByCodesAsync(productCodes);
                
                // Create check functions using loaded data
                Func<OrderNumber, (bool exists, OrderDetails? details)> checkOrderExists = 
                    orderNumber => _ordersRepository.GetOrderByNumberAsync(orderNumber).Result;
                
                Func<ProductCode, (bool exists, string productName, decimal price)> checkProductCatalog = 
                    productCode => _productsRepository.GetProductByCodeAsync(productCode).Result;
                
                Func<ProductCode, Quantity, bool> checkInventory = 
                    (productCode, quantity) => _productsRepository.CheckStockAsync(productCode, quantity).Result;

                // STEP 2: EXECUTE PURE BUSINESS LOGIC (no database access!)
                IModifyOrderRequest request = ExecuteBusinessLogic(command, checkOrderExists, checkProductCatalog, checkInventory);

                // STEP 3: SAVE RESULTS TO DATABASE
                if (request is ModifiedOrder modifiedOrder)
                {
                    await _ordersRepository.UpdateOrderAsync(modifiedOrder.OrderNumber, modifiedOrder.NewTotalPrice);
                }

                // Convert final state to event
                return request.ToEvent();
            }
            catch (Exception ex)
            {
                return new OrderModifiedFailedEvent(new[] { $"Unexpected error: {ex.Message}" });
            }
        }

        private static IModifyOrderRequest ExecuteBusinessLogic(
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
            
            // Execute the workflow operations pipeline (pure functions)
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
            
            return request;
        }
    }
}