<<<<<<< HEAD
using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
=======
﻿using StackBook.Models;
using StackBook.Services;
>>>>>>> 5a5bf7a (feat-order)

namespace StackBook.DAL.IRepository
{
    public interface IOrderRepository
    {
<<<<<<< HEAD
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
=======
        //Create new Order
        Task<Order> CreateOrderAsync(Guid userId, Guid discountId, Guid shippingAddressId, double totalPrice, int status);

        //Get all Orders
        Task<List<Order>> GetAllOrdersAsync(Guid userId);

        //Get Orser By Id
        Task<Order?> GetOrderByIdAsync(Guid orderId);

        //Update Status Order
        Task<Order> UpdateOrderStatusByIdAsync(Guid orderId, int status);

    }
}
>>>>>>> 5a5bf7a (feat-order)
