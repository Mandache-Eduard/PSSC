using System.Collections.Generic;
namespace OrderManagement.Domain.Models
{
    public record ReturnOrderCommand
    {
        public ReturnOrderCommand(string orderNumber, string returnReason, IReadOnlyCollection<UnvalidatedReturnItem> returnItems)
        {
            OrderNumber = orderNumber;
            ReturnReason = returnReason;
            ReturnItems = returnItems;
        }
        public string OrderNumber { get; }
        public string ReturnReason { get; }
        public IReadOnlyCollection<UnvalidatedReturnItem> ReturnItems { get; }
    }
}