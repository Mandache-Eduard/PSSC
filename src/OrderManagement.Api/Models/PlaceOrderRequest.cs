using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Api.Models
{
    /// <summary>
    /// Request model for placing a new order
    /// </summary>
    public class PlaceOrderRequest
    {
        /// <summary>
        /// List of products to order
        /// </summary>
        [Required(ErrorMessage = "Order lines are required")]
        [MinLength(1, ErrorMessage = "At least one order line is required")]
        public List<InputOrderLine> OrderLines { get; set; } = new();

        /// <summary>
        /// Street address for shipping
        /// </summary>
        [Required(ErrorMessage = "Street is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Street must be between 5 and 200 characters")]
        public string Street { get; set; } = string.Empty;

        /// <summary>
        /// City for shipping
        /// </summary>
        [Required(ErrorMessage = "City is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Postal code for shipping
        /// </summary>
        [Required(ErrorMessage = "Postal code is required")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid postal code format")]
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Country for shipping
        /// </summary>
        [Required(ErrorMessage = "Country is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 100 characters")]
        public string Country { get; set; } = string.Empty;
    }
}
