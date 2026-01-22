using OrderManagement.Domain.Models;
using OrderManagement.Domain.Operations;
using OrderManagement.Domain.Repositories;
using System;
using System.Threading.Tasks;
using static OrderManagement.Domain.Models.ReturnOrderRequest;
using static OrderManagement.Domain.Models.OrderReturnedEvent;

namespace OrderManagement.Domain.Workflows
{
    public class ReturnOrderWorkflow
    {
        private readonly IOrdersRepository _ordersRepository;

        public ReturnOrderWorkflow(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task<IOrderReturnedEvent> ExecuteAsync(ReturnOrderCommand command)
        {
            try
            {
                // STEP 1: LOAD STATE FROM DATABASE
                // Create check function using database
                Func<OrderNumber, (bool exists, OrderDetails? details)> checkOrderExists = 
                    orderNumber => _ordersRepository.GetOrderByNumberAsync(orderNumber).Result;

                // STEP 2: EXECUTE PURE BUSINESS LOGIC (no database access!)
                IReturnOrderRequest request = ExecuteBusinessLogic(command, checkOrderExists);

                // STEP 3: SAVE RESULTS TO DATABASE
                if (request is ProcessedReturn processedReturn)
                {
                    await _ordersRepository.UpdateOrderStatusAsync(processedReturn.OrderNumber, "Returned");
                }

                // Convert final state to event
                return request.ToEvent();
            }
            catch (Exception ex)
            {
                return new OrderReturnedFailedEvent(new[] { $"Unexpected error: {ex.Message}" });
            }
        }

        private static IReturnOrderRequest ExecuteBusinessLogic(
            ReturnOrderCommand command,
            Func<OrderNumber, (bool exists, OrderDetails? details)> checkOrderExists)
        {
            // Create unvalidated return request from command
            UnvalidatedReturnRequest unvalidatedRequest = new(
                command.OrderNumber,
                command.ReturnReason,
                command.ReturnItems
            );
            
            // Execute the workflow operations pipeline (pure functions)
            IReturnOrderRequest request = unvalidatedRequest;
            
            // 1. Validate input data (order number, return reason, return items)
            request = new ValidateReturnRequestOperation().Transform(request);
            
            // 2. Verify order exists and can be returned (status check, 14-day window)
            request = new VerifyOrderCanBeReturnedOperation(checkOrderExists).Transform(request);
            
            // 3. Calculate refund based on return reason type (shipping fee logic)
            request = new CalculateReturnRefundOperation().Transform(request);
            
            // 4. Process the return (generate return number, finalize)
            request = new ProcessReturnOperation().Transform(request);
            
            return request;
        }
    }
}