using OrderManagement.Domain.Exceptions;
using static OrderManagement.Domain.Models.CancelOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal abstract class CancelOrderOperation<TState> : DomainOperation<ICancelOrderRequest, TState, ICancelOrderRequest>
        where TState : class
    {
        public override ICancelOrderRequest Transform(ICancelOrderRequest request, TState? state) => request switch
        {
            UnvalidatedCancelRequest unvalidated => OnUnvalidated(unvalidated, state),
            InvalidCancelRequest invalid => OnInvalid(invalid, state),
            ValidatedCancelRequest validated => OnValidated(validated, state),
            OrderVerifiedCancelRequest orderVerified => OnOrderVerified(orderVerified, state),
            RefundCalculatedCancelRequest refundCalculated => OnRefundCalculated(refundCalculated, state),
            CancelledOrder cancelled => OnCancelled(cancelled, state),
            _ => throw new InvalidOrderStateException(request.GetType().Name)
        };
        protected virtual ICancelOrderRequest OnUnvalidated(UnvalidatedCancelRequest request, TState? state) => request;
        protected virtual ICancelOrderRequest OnInvalid(InvalidCancelRequest request, TState? state) => request;
        protected virtual ICancelOrderRequest OnValidated(ValidatedCancelRequest request, TState? state) => request;
        protected virtual ICancelOrderRequest OnOrderVerified(OrderVerifiedCancelRequest request, TState? state) => request;
        protected virtual ICancelOrderRequest OnRefundCalculated(RefundCalculatedCancelRequest request, TState? state) => request;
        protected virtual ICancelOrderRequest OnCancelled(CancelledOrder request, TState? state) => request;
    }
    internal abstract class CancelOrderOperation : CancelOrderOperation<object>
    {
        internal ICancelOrderRequest Transform(ICancelOrderRequest request) => Transform(request, null);
        protected sealed override ICancelOrderRequest OnUnvalidated(UnvalidatedCancelRequest request, object? state) => OnUnvalidated(request);
        protected virtual ICancelOrderRequest OnUnvalidated(UnvalidatedCancelRequest request) => request;
        protected sealed override ICancelOrderRequest OnInvalid(InvalidCancelRequest request, object? state) => OnInvalid(request);
        protected virtual ICancelOrderRequest OnInvalid(InvalidCancelRequest request) => request;
        protected sealed override ICancelOrderRequest OnValidated(ValidatedCancelRequest request, object? state) => OnValidated(request);
        protected virtual ICancelOrderRequest OnValidated(ValidatedCancelRequest request) => request;
        protected sealed override ICancelOrderRequest OnOrderVerified(OrderVerifiedCancelRequest request, object? state) => OnOrderVerified(request);
        protected virtual ICancelOrderRequest OnOrderVerified(OrderVerifiedCancelRequest request) => request;
        protected sealed override ICancelOrderRequest OnRefundCalculated(RefundCalculatedCancelRequest request, object? state) => OnRefundCalculated(request);
        protected virtual ICancelOrderRequest OnRefundCalculated(RefundCalculatedCancelRequest request) => request;
        protected sealed override ICancelOrderRequest OnCancelled(CancelledOrder request, object? state) => OnCancelled(request);
        protected virtual ICancelOrderRequest OnCancelled(CancelledOrder request) => request;
    }
}