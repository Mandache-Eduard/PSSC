namespace OrderManagement.Domain.Models
{
    public record CancelOrderCommand
    {
        public CancelOrderCommand(string orderNumber, string reason)
        {
            OrderNumber = orderNumber;
            Reason = reason;
        }
        public string OrderNumber { get; }
        public string Reason { get; }
    }
}