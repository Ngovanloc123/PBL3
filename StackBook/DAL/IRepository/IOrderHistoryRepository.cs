using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.DAL.IRepository
{
    public interface IOrderHistoryRepository : IRepository<OrderHistory>
    {
        Task AddOrderHistoryAsync(OrderHistory orderHistory);
        Task<List<OrderHistory>> GetOrderHistoryAsync(Guid orderId);
        Task SaveChangesAsync();
    }
}