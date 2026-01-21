using OrderManagement.Domain.Exceptions;
using static OrderManagement.Domain.Models.Order;
namespace OrderManagement.Domain.Operations
{
    internal abstract class OrderOperation<TState> : DomainOperation<IOrder, TState, IOrder>
        where TState : class
    {
        public override IOrder Transform(IOrder order, TState? state) => order switch
        {
            UnvalidatedOrder unvalidatedOrder => OnUnvalidated(unvalidatedOrder, state),
            ValidatedOrder validatedOrder => OnValidated(validatedOrder, state),
            InvalidOrder invalidOrder => OnInvalid(invalidOrder, state),
            ProductVerifiedOrder productVerifiedOrder => OnProductVerified(productVerifiedOrder, state),
            PricedOrder pricedOrder => OnPriced(pricedOrder, state),
            ConfirmedOrder confirmedOrder => OnConfirmed(confirmedOrder, state),
            _ => throw new InvalidOrderStateException(order.GetType().Name)
        };
        protected virtual IOrder OnUnvalidated(UnvalidatedOrder order, TState? state) => order;
        protected virtual IOrder OnValidated(ValidatedOrder order, TState? state) => order;
        protected virtual IOrder OnInvalid(InvalidOrder order, TState? state) => order;
        protected virtual IOrder OnProductVerified(ProductVerifiedOrder order, TState? state) => order;
        protected virtual IOrder OnPriced(PricedOrder order, TState? state) => order;
        protected virtual IOrder OnConfirmed(ConfirmedOrder order, TState? state) => order;
    }
    internal abstract class OrderOperation : OrderOperation<object>
    {
        internal IOrder Transform(IOrder order) => Transform(order, null);
        protected sealed override IOrder OnUnvalidated(UnvalidatedOrder order, object? state) => OnUnvalidated(order);
        protected virtual IOrder OnUnvalidated(UnvalidatedOrder order) => order;
        protected sealed override IOrder OnValidated(ValidatedOrder order, object? state) => OnValidated(order);
        protected virtual IOrder OnValidated(ValidatedOrder order) => order;
        protected sealed override IOrder OnInvalid(InvalidOrder order, object? state) => OnInvalid(order);
        protected virtual IOrder OnInvalid(InvalidOrder order) => order;
        protected sealed override IOrder OnProductVerified(ProductVerifiedOrder order, object? state) => OnProductVerified(order);
        protected virtual IOrder OnProductVerified(ProductVerifiedOrder order) => order;
        protected sealed override IOrder OnPriced(PricedOrder order, object? state) => OnPriced(order);
        protected virtual IOrder OnPriced(PricedOrder order) => order;
        protected sealed override IOrder OnConfirmed(ConfirmedOrder order, object? state) => OnConfirmed(order);
        protected virtual IOrder OnConfirmed(ConfirmedOrder order) => order;
    }
}