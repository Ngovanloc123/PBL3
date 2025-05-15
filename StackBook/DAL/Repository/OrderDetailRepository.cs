using StackBook.Models;
using StackBook.DAL.IRepository;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.DAL.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<OrderDetail?> GetByIdAsync(Guid orderDetailId)
        {
            return await _db.OrderDetails
                .Include(od => od.Book)
                .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);
        }

        public async Task<List<OrderDetail>> GetByOrderIdAsync(Guid orderId)
        {
            return await _db.OrderDetails
                .Include(od => od.Book)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            await _db.OrderDetails.AddAsync(orderDetail);
        }

        public async Task UpdateAsync(OrderDetail orderDetail)
        {
            _db.OrderDetails.Update(orderDetail);
        }

        public async Task DeleteAsync(OrderDetail orderDetail)
        {
            _db.OrderDetails.Remove(orderDetail);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}