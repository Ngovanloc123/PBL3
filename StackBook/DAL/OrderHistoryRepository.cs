using StackBook.Models;
using StackBook.DAL.IRepository;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.DAL
{
    public class OrderHistoryRepository : IOrderHistoryRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHistoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddOrderHistoryAsync(OrderHistory orderHistory)
        {
            await _db.OrderHistories.AddAsync(orderHistory);
        }

        public async Task<List<OrderHistory>> GetOrderHistoryAsync(Guid orderId)
        {
            return await _db.OrderHistories
                .Where(oh => oh.OrderId == orderId)
                .OrderByDescending(oh => oh.createdStatus)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}