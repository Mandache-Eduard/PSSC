namespace OrderManagement.Data.Models
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
        
        // Navigation properties
        public OrderDto? Order { get; set; }
        public ProductDto? Product { get; set; }
    }
}

