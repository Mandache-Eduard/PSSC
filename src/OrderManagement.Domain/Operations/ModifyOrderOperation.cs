using OrderManagement.Domain.Exceptions;
using static OrderManagement.Domain.Models.ModifyOrderRequest;
namespace OrderManagement.Domain.Operations
{
    internal abstract class ModifyOrderOperation<TState> : DomainOperation<IModifyOrderRequest, TState, IModifyOrderRequest>
        where TState : class
    {
        public override IModifyOrderRequest Transform(IModifyOrderRequest request, TState? state) => request switch
        {
            UnvalidatedModifyRequest unvalidated => OnUnvalidated(unvalidated, state),
            InvalidModifyRequest invalid => OnInvalid(invalid, state),
            ValidatedModifyRequest validated => OnValidated(validated, state),
            OrderVerifiedModifyRequest orderVerified => OnOrderVerified(orderVerified, state),
            ProductsVerifiedModifyRequest productsVerified => OnProductsVerified(productsVerified, state),
            PriceRecalculatedModifyRequest priceRecalculated => OnPriceRecalculated(priceRecalculated, state),
            ModifiedOrder modified => OnModified(modified, state),
            _ => throw new InvalidOrderStateException(request.GetType().Name)
        };
        protected virtual IModifyOrderRequest OnUnvalidated(UnvalidatedModifyRequest request, TState? state) => request;
        protected virtual IModifyOrderRequest OnInvalid(InvalidModifyRequest request, TState? state) => request;
        protected virtual IModifyOrderRequest OnValidated(ValidatedModifyRequest request, TState? state) => request;
        protected virtual IModifyOrderRequest OnOrderVerified(OrderVerifiedModifyRequest request, TState? state) => request;
        protected virtual IModifyOrderRequest OnProductsVerified(ProductsVerifiedModifyRequest request, TState? state) => request;
        protected virtual IModifyOrderRequest OnPriceRecalculated(PriceRecalculatedModifyRequest request, TState? state) => request;
        protected virtual IModifyOrderRequest OnModified(ModifiedOrder request, TState? state) => request;
    }
    internal abstract class ModifyOrderOperation : ModifyOrderOperation<object>
    {
        internal IModifyOrderRequest Transform(IModifyOrderRequest request) => Transform(request, null);
        protected sealed override IModifyOrderRequest OnUnvalidated(UnvalidatedModifyRequest request, object? state) => OnUnvalidated(request);
        protected virtual IModifyOrderRequest OnUnvalidated(UnvalidatedModifyRequest request) => request;
        protected sealed override IModifyOrderRequest OnInvalid(InvalidModifyRequest request, object? state) => OnInvalid(request);
        protected virtual IModifyOrderRequest OnInvalid(InvalidModifyRequest request) => request;
        protected sealed override IModifyOrderRequest OnValidated(ValidatedModifyRequest request, object? state) => OnValidated(request);
        protected virtual IModifyOrderRequest OnValidated(ValidatedModifyRequest request) => request;
        protected sealed override IModifyOrderRequest OnOrderVerified(OrderVerifiedModifyRequest request, object? state) => OnOrderVerified(request);
        protected virtual IModifyOrderRequest OnOrderVerified(OrderVerifiedModifyRequest request) => request;
        protected sealed override IModifyOrderRequest OnProductsVerified(ProductsVerifiedModifyRequest request, object? state) => OnProductsVerified(request);
        protected virtual IModifyOrderRequest OnProductsVerified(ProductsVerifiedModifyRequest request) => request;
        protected sealed override IModifyOrderRequest OnPriceRecalculated(PriceRecalculatedModifyRequest request, object? state) => OnPriceRecalculated(request);
        protected virtual IModifyOrderRequest OnPriceRecalculated(PriceRecalculatedModifyRequest request) => request;
        protected sealed override IModifyOrderRequest OnModified(ModifiedOrder request, object? state) => OnModified(request);
        protected virtual IModifyOrderRequest OnModified(ModifiedOrder request) => request;
    }
}