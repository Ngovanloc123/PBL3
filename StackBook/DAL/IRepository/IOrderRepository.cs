using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace StackBook.DAL.IRepository
{
    public interface IOrderRepository
    {
        Task CreateOrderAsync(Order order);
        Task<Order?> FindOrderByIdAsync(Guid orderId);
        Task<List<Order>> FindOrdersByUserIdAsync(Guid userId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetOrdersByStatusAsync(int status);
        Task UpdateOrderStatusAsync(Guid orderId, int status);
        Task UpdateAddressAsync(Guid orderId, string address);
        Task DeleteOrderAsync(Guid orderId);
        Task SaveChangesAsync();
    }

    public interface IOrderDetailsRepository
    {
        //Create new OrderDetails
        Task CreateOrderDetailsAsync(OrderDetail orderDetail);

        //Get all OrderDetails in Order by orderId
        Task<List<OrderDetail>> GetAllOrderDetailsAsync(Guid orderId);

        //Get OrderDetail by orderDetailId
        Task<OrderDetail?> GetOrderDetailByIdAsync(Guid orderDetailId);
    }
}

