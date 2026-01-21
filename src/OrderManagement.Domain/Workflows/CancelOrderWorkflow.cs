using OrderManagement.Domain.Models;
using OrderManagement.Domain.Operations;
using System;
using static OrderManagement.Domain.Models.CancelOrderRequest;
using static OrderManagement.Domain.Models.OrderCancelledEvent;
namespace OrderManagement.Domain.Workflows
{
    public class CancelOrderWorkflow
    {
        public IOrderCancelledEvent Execute(
            CancelOrderCommand command,
            Func<OrderNumber, (bool exists, OrderDetails? details)> checkOrderExists)
        {
            // Create unvalidated cancel request from command
            UnvalidatedCancelRequest unvalidatedRequest = new(
                command.OrderNumber,
                command.Reason
            );
            // Execute the workflow operations pipeline
            ICancelOrderRequest request = unvalidatedRequest;
            // 1. Validate input data (order number format, reason length)
            request = new ValidateCancelRequestOperation().Transform(request);
            // 2. Verify order exists and can be cancelled (business rule: only confirmed orders)
            request = new VerifyOrderExistsOperation(checkOrderExists).Transform(request);
            // 3. Calculate refund based on cancellation timing (business rules)
            request = new CalculateRefundOperation().Transform(request);
            // 4. Process the cancellation (finalize)
            request = new ProcessCancellationOperation().Transform(request);
            // Convert final state to event
            return request.ToEvent();
        }
    }
}