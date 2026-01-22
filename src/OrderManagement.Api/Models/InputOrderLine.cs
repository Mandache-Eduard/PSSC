using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Api.Models
{
    /// <summary>
    /// Represents an order line for API input
    /// </summary>
    public class InputOrderLine
    {
        /// <summary>
        /// Product code (e.g., AB1234)
        /// </summary>
        [Required(ErrorMessage = "Product code is required")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "Product code must be between 6 and 10 characters")]
        public string ProductCode { get; set; } = string.Empty;

        /// <summary>
        /// Quantity to order
        /// </summary>
        [Required(ErrorMessage = "Quantity is required")]
        [Range(0.1, 1000, ErrorMessage = "Quantity must be between 0.1 and 1000")]
        public decimal Quantity { get; set; }
    }
}
