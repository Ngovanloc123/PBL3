using StackBook.Models;
using StackBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Guid userId, CheckoutRequest request);
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<List<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetOrdersByStatusAsync(int status);
        Task UpdateOrderStatusAsync(Guid orderId, int status);
        Task UpdateShippingAddressAsync(Guid orderId, Guid shippingAddressId);
        Task CancelOrderAsync(Guid orderId);
        Task<List<OrderDetail>> GetOrderDetailsAsync(Guid orderId);
        Task<List<OrderHistory>> GetOrderHistoryAsync(Guid orderId);
    }
}