using OrderManagement.Domain.Models;
using static OrderManagement.Domain.Models.CancelOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal sealed class ValidateCancelRequestOperation : CancelOrderOperation
    {
        protected override ICancelOrderRequest OnUnvalidated(UnvalidatedCancelRequest request)
        {
            // Validate order number
            if (!OrderNumber.TryParse(request.OrderNumber, out OrderNumber? orderNumber))
            {
                return new InvalidCancelRequest(
                    request.OrderNumber,
                    request.Reason,
                    $"Invalid order number format: {request.OrderNumber}. Expected format: ORD-YYYYMMDD-XXXXXXXX"
                );
            }
            // Validate cancellation reason
            if (!CancellationReason.TryParse(request.Reason, out CancellationReason? reason))
            {
                return new InvalidCancelRequest(
                    request.OrderNumber,
                    request.Reason,
                    "Cancellation reason must be at least 10 characters long"
                );
            }
            // Return validated request
            return new ValidatedCancelRequest(orderNumber!, reason!);
        }
    }
}