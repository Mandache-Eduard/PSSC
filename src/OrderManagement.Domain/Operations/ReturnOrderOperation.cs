using OrderManagement.Domain.Exceptions;
using static OrderManagement.Domain.Models.ReturnOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal abstract class ReturnOrderOperation<TState> : DomainOperation<IReturnOrderRequest, TState, IReturnOrderRequest>
        where TState : class
    {
        public override IReturnOrderRequest Transform(IReturnOrderRequest request, TState? state) => request switch
        {
            UnvalidatedReturnRequest unvalidated => OnUnvalidated(unvalidated, state),
            InvalidReturnRequest invalid => OnInvalid(invalid, state),
            ValidatedReturnRequest validated => OnValidated(validated, state),
            OrderVerifiedReturnRequest orderVerified => OnOrderVerified(orderVerified, state),
            ReturnApprovedRequest returnApproved => OnReturnApproved(returnApproved, state),
            ProcessedReturn processed => OnProcessed(processed, state),
            _ => throw new InvalidOrderStateException(request.GetType().Name)
        };
        protected virtual IReturnOrderRequest OnUnvalidated(UnvalidatedReturnRequest request, TState? state) => request;
        protected virtual IReturnOrderRequest OnInvalid(InvalidReturnRequest request, TState? state) => request;
        protected virtual IReturnOrderRequest OnValidated(ValidatedReturnRequest request, TState? state) => request;
        protected virtual IReturnOrderRequest OnOrderVerified(OrderVerifiedReturnRequest request, TState? state) => request;
        protected virtual IReturnOrderRequest OnReturnApproved(ReturnApprovedRequest request, TState? state) => request;
        protected virtual IReturnOrderRequest OnProcessed(ProcessedReturn request, TState? state) => request;
    }
    internal abstract class ReturnOrderOperation : ReturnOrderOperation<object>
    {
        internal IReturnOrderRequest Transform(IReturnOrderRequest request) => Transform(request, null);
        protected sealed override IReturnOrderRequest OnUnvalidated(UnvalidatedReturnRequest request, object? state) => OnUnvalidated(request);
        protected virtual IReturnOrderRequest OnUnvalidated(UnvalidatedReturnRequest request) => request;
        protected sealed override IReturnOrderRequest OnInvalid(InvalidReturnRequest request, object? state) => OnInvalid(request);
        protected virtual IReturnOrderRequest OnInvalid(InvalidReturnRequest request) => request;
        protected sealed override IReturnOrderRequest OnValidated(ValidatedReturnRequest request, object? state) => OnValidated(request);
        protected virtual IReturnOrderRequest OnValidated(ValidatedReturnRequest request) => request;
        protected sealed override IReturnOrderRequest OnOrderVerified(OrderVerifiedReturnRequest request, object? state) => OnOrderVerified(request);
        protected virtual IReturnOrderRequest OnOrderVerified(OrderVerifiedReturnRequest request) => request;
        protected sealed override IReturnOrderRequest OnReturnApproved(ReturnApprovedRequest request, object? state) => OnReturnApproved(request);
        protected virtual IReturnOrderRequest OnReturnApproved(ReturnApprovedRequest request) => request;
        protected sealed override IReturnOrderRequest OnProcessed(ProcessedReturn request, object? state) => OnProcessed(request);
        protected virtual IReturnOrderRequest OnProcessed(ProcessedReturn request) => request;
    }
}