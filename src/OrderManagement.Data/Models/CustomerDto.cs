namespace OrderManagement.Data.Models
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

