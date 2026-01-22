using Microsoft.EntityFrameworkCore;
using OrderManagement.Data.Models;
using OrderManagement.Domain.Models;
using OrderManagement.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OrderManagement.Domain.Models.CancelOrderRequest;
using static OrderManagement.Domain.Models.Order;

namespace OrderManagement.Data.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly OrderManagementContext _context;

        public OrdersRepository(OrderManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderSummary>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderSummary(
                    o.OrderNumber,
                    o.TotalAmount,
                    o.Status,
                    o.OrderDate,
                    o.City,
                    o.Country
                ))
                .ToListAsync();

            return orders;
        }

        public async Task<(bool exists, OrderDetails? details)> GetOrderByNumberAsync(OrderNumber orderNumber)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber.Value);

            if (order == null)
            {
                return (false, null);
            }

            // Create OrderDetails using TryCreate
            if (OrderDetails.TryCreate(order.TotalAmount, order.OrderDate, order.Status, out var orderDetails))
            {
                return (true, orderDetails);
            }

            return (false, null);
        }

        public async Task SaveOrderAsync(string orderNumber, decimal totalAmount, string status, string street, string city, string postalCode, string country)
        {
            var order = new OrderDto
            {
                OrderNumber = orderNumber,
                TotalAmount = totalAmount,
                Status = status,
                Street = street,
                City = city,
                PostalCode = postalCode,
                Country = country,
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task SaveCompleteOrderAsync(ConfirmedOrder order)
        {
            // Create the order DTO
            var orderDto = new OrderDto
            {
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalPrice,
                Status = "Confirmed",
                Street = order.ShippingAddress.Street,
                City = order.ShippingAddress.City,
                PostalCode = order.ShippingAddress.PostalCode,
                Country = order.ShippingAddress.Country,
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(orderDto);
            await _context.SaveChangesAsync(); // Save to get OrderId

            // Create order items
            foreach (var orderLine in order.OrderLines)
            {
                // Get product by code to get ProductId
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Code == orderLine.ProductCode.Value);

                if (product != null)
                {
                    var orderItemDto = new OrderItemDto
                    {
                        OrderId = orderDto.OrderId,
                        ProductId = product.ProductId,
                        Quantity = orderLine.Quantity.Value,
                        Price = orderLine.Price,
                        LineTotal = orderLine.LineTotal
                    };

                    _context.OrderItems.Add(orderItemDto);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderStatusAsync(OrderNumber orderNumber, string status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber.Value);

            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateOrderAsync(OrderNumber orderNumber, decimal newTotalAmount)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber.Value);

            if (order != null)
            {
                order.TotalAmount = newTotalAmount;
                await _context.SaveChangesAsync();
            }
        }
    }
}

