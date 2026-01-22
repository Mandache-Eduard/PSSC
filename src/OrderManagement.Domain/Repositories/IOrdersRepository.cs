using OrderManagement.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OrderManagement.Domain.Models.CancelOrderRequest;
using static OrderManagement.Domain.Models.Order;

namespace OrderManagement.Domain.Repositories
{
    public interface IOrdersRepository
    {
        Task<IEnumerable<OrderSummary>> GetAllOrdersAsync();
        Task<(bool exists, OrderDetails? details)> GetOrderByNumberAsync(OrderNumber orderNumber);
        Task SaveOrderAsync(string orderNumber, decimal totalAmount, string status, string street, string city, string postalCode, string country);
        Task SaveCompleteOrderAsync(ConfirmedOrder order);
        Task UpdateOrderStatusAsync(OrderNumber orderNumber, string status);
        Task UpdateOrderAsync(OrderNumber orderNumber, decimal newTotalAmount);
    }
    
    /// <summary>
    /// Summary information about an order for list views
    /// </summary>
    public record OrderSummary(
        string OrderNumber,
        decimal TotalAmount,
        string Status,
        DateTime OrderDate,
        string ShippingCity,
        string ShippingCountry
    );
}
