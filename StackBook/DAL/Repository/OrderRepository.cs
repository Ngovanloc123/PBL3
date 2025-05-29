using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackBook.DAL.IRepository;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;
using StackBook.VMs;

namespace StackBook.DAL.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public async Task CreateOrderAsync(Order order)
        {
            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();
        }
        public async Task<Order?> FindOrderByIdAsync(Guid orderId)
        {
            return await _db.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Payments)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
        public async Task<List<Order>> FindOrdersByUserIdAsync(Guid userId)
        {
            return await _db.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Payments)
                .Include(o => o.ShippingAddress)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _db.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Payments)
                .Include(o => o.ShippingAddress)
                .ToListAsync();
        }
        public async Task<List<Order>> GetOrdersByStatusAsync(int status)
        {
            return await _db.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Payments)
                .Include(o => o.ShippingAddress)
                .Where(o => o.Status == status)
                .ToListAsync();
        }
        public async Task<List<OrderDetail>> GetOrderDetailsAsync(Guid orderId)
        {
            return await _db.OrderDetails
                .Include(od => od.Book)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }
        public async Task<List<OrderDetailVM>> GetOrderDetailsDtoAsync(Guid orderId)
        {
            var orderDetails = await _db.OrderDetails
                .Include(od => od.Book)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
            var orderDetailDtos = new List<OrderDetailVM>();
            foreach (var od in orderDetails)
            {
                orderDetailDtos.Add(new OrderDetailVM
                {
                    OrderDetailId = od.OrderDetailId,
                    BookId = od.BookId,
                    BookTitle = od.Book.BookTitle.ToString(),
                    Quantity = od.Quantity,
                    Price = od.Book.Price,
                    TotalPrice = od.Quantity * od.Book.Price
                });
            }
            return orderDetailDtos;
        }
        public async Task UpdateOrderStatusAsync(Guid orderId, int status)
        {
            var order = await FindOrderByIdAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();
            }
        }
        public async Task UpdateAddressAsync(Guid orderId, string address)
        {
            var order = await FindOrderByIdAsync(orderId);
            if (order != null)
            {
                if(order.ShippingAddress != null)
                {
                    order.ShippingAddress.Address = address;
                }
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();
            }
        }
        public async Task DeleteOrderAsync(Guid orderId)
        {
            var order = await FindOrderByIdAsync(orderId);
            if (order != null)
            {
                _db.Orders.Remove(order);
                await _db.SaveChangesAsync();
            }
        }
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}