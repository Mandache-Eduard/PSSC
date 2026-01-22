namespace OrderManagement.Data.Models
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        
        // Navigation properties
        public CustomerDto? Customer { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}

