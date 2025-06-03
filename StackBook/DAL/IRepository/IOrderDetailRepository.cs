using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.DAL.IRepository
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail?> GetByIdAsync(Guid orderDetailId);
        Task<List<OrderDetail>> GetByOrderIdAsync(Guid orderId);
        Task AddAsync(OrderDetail orderDetail);
        Task UpdateAsync(OrderDetail orderDetail);
        Task DeleteAsync(OrderDetail orderDetail);
        Task SaveChangesAsync();
        Task<bool> CanUserReviewBookAsync(Guid userId, Guid bookId);
    }
}