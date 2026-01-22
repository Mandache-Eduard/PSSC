using OrderManagement.Domain.Models;
using OrderManagement.Domain.Operations;
using OrderManagement.Domain.Repositories;
using System;
using System.Threading.Tasks;
using static OrderManagement.Domain.Models.CancelOrderRequest;
using static OrderManagement.Domain.Models.OrderCancelledEvent;

namespace OrderManagement.Domain.Workflows
{
    public class CancelOrderWorkflow
    {
        private readonly IOrdersRepository _ordersRepository;

        public CancelOrderWorkflow(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task<IOrderCancelledEvent> ExecuteAsync(CancelOrderCommand command)
        {
            try
            {
                // STEP 1: LOAD STATE FROM DATABASE
                // Create check function using database
                Func<OrderNumber, (bool exists, OrderDetails? details)> checkOrderExists = 
                    orderNumber => _ordersRepository.GetOrderByNumberAsync(orderNumber).Result;

                // STEP 2: EXECUTE PURE BUSINESS LOGIC (no database access!)
                ICancelOrderRequest request = ExecuteBusinessLogic(command, checkOrderExists);

                // STEP 3: SAVE RESULTS TO DATABASE
                if (request is CancelledOrder cancelledOrder)
                {
                    await _ordersRepository.UpdateOrderStatusAsync(cancelledOrder.OrderNumber, "Cancelled");
                }

                // Convert final state to event
                return request.ToEvent();
            }
            catch (Exception ex)
            {
                return new OrderCancelledFailedEvent($"Unexpected error: {ex.Message}");
            }
        }

        private static ICancelOrderRequest ExecuteBusinessLogic(
            CancelOrderCommand command,
            Func<OrderNumber, (bool exists, OrderDetails? details)> checkOrderExists)
        {
            // Create unvalidated cancel request from command
            UnvalidatedCancelRequest unvalidatedRequest = new(
                command.OrderNumber,
                command.Reason
            );
            
            // Execute the workflow operations pipeline (pure functions)
            ICancelOrderRequest request = unvalidatedRequest;
            
            // 1. Validate input data (order number format, reason length)
            request = new ValidateCancelRequestOperation().Transform(request);
            
            // 2. Verify order exists and can be cancelled (business rule: only confirmed orders)
            request = new VerifyOrderExistsOperation(checkOrderExists).Transform(request);
            
            // 3. Calculate refund based on cancellation timing (business rules)
            request = new CalculateRefundOperation().Transform(request);
            
            // 4. Process the cancellation (finalize)
            request = new ProcessCancellationOperation().Transform(request);
            
            return request;
        }
    }
}