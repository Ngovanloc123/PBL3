using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace StackBook.DAL.IRepository
{
    public interface IOrderRepository
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
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangesAsync();
    }
}