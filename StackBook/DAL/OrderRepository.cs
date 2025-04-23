<<<<<<< HEAD
using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackBook.DAL.IRepository;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;
=======
﻿using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;
>>>>>>> 5a5bf7a (feat-order)

namespace StackBook.DAL
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

<<<<<<< HEAD
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
        public async Task<List<OrderDetailDto>> GetOrderDetailsDtoAsync(Guid orderId)
        {
            var orderDetails = await _db.OrderDetails
                .Include(od => od.Book)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
            var orderDetailDtos = new List<OrderDetailDto>();
            foreach (var od in orderDetails)
            {
                orderDetailDtos.Add(new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    BookId = od.BookId,
                    BookTitle = od.Book.BookTitle,
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
=======
        // Tạo đơn hàng mới
        public async Task<Order> CreateOrderAsync(Guid userId, Guid discountId, Guid shippingAddressId, double totalPrice, int status)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                UserId = userId,
                DiscountId = discountId,
                ShippingAddressId = shippingAddressId,
                TotalPrice = totalPrice,
                Status = status
            };

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();
            return order;
        }

        // Lấy danh sách tất cả đơn hàng của một người dùng
        public async Task<List<Order>> GetAllOrdersAsync(Guid userId)
        {
            return await _db.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .ToListAsync();
        }

        // Lấy thông tin đơn hàng theo ID
        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _db.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        // Cập nhật trạng thái đơn hàng
        public async Task<Order> UpdateOrderStatusByIdAsync(Guid orderId, int status)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null)
                return null;

            order.Status = status;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
            return order;
        }
    }
}
>>>>>>> 5a5bf7a (feat-order)
