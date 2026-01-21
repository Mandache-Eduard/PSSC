using System.Collections.Generic;
namespace OrderManagement.Domain.Models
{
    public record ModifyOrderCommand
    {
        public ModifyOrderCommand(string orderNumber, IReadOnlyCollection<UnvalidatedOrderLine> newOrderLines)
        {
            OrderNumber = orderNumber;
            NewOrderLines = newOrderLines;
        }
        public string OrderNumber { get; }
        public IReadOnlyCollection<UnvalidatedOrderLine> NewOrderLines { get; }
    }
}