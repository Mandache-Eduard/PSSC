using OrderManagement.Domain.Models;
using OrderManagement.Domain.Operations;
using System;
using static OrderManagement.Domain.Models.ReturnOrderRequest;
using static OrderManagement.Domain.Models.OrderReturnedEvent;
namespace OrderManagement.Domain.Workflows
{
    public class ReturnOrderWorkflow
    {
        public IOrderReturnedEvent Execute(
            ReturnOrderCommand command,
            Func<OrderNumber, (bool exists, OrderDetails? details)> checkOrderExists)
        {
            // Create unvalidated return request from command
            UnvalidatedReturnRequest unvalidatedRequest = new(
                command.OrderNumber,
                command.ReturnReason,
                command.ReturnItems
            );
            // Execute the workflow operations pipeline
            IReturnOrderRequest request = unvalidatedRequest;
            // 1. Validate input data (order number, return reason, return items)
            request = new ValidateReturnRequestOperation().Transform(request);
            // 2. Verify order exists and can be returned (status check, 30-day window)
            request = new VerifyOrderCanBeReturnedOperation(checkOrderExists).Transform(request);
            // 3. Calculate refund based on return reason type (restocking fee logic)
            request = new CalculateReturnRefundOperation().Transform(request);
            // 4. Process the return (generate return number, finalize)
            request = new ProcessReturnOperation().Transform(request);
            // Convert final state to event
            return request.ToEvent();
        }
    }
}