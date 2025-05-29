using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.DAL.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task CreateOrderAsync(Order order);
        Task<Order?> FindOrderByIdAsync(Guid orderId);
        Task<List<Order>> FindOrdersByUserIdAsync(Guid userId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetOrdersByStatusAsync(int status);
        Task<List<OrderDetail>> GetOrderDetailsAsync(Guid orderId);
        Task UpdateOrderStatusAsync(Guid orderId, int status);
        Task UpdateAddressAsync(Guid orderId, string address);
        Task DeleteOrderAsync(Guid orderId);
        Task SaveChangesAsync();
    }
}