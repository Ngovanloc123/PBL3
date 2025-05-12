using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;
using StackBook.Exceptions;
using StackBook.DAL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackBook.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly ICartService _cartService;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IShippingAddressRepository _shippingAddressRepository;
        private readonly IOrderHistoryRepository _orderHistoryRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IDiscountRepository discountRepository,
            ICartService cartService,
            IOrderDetailRepository orderDetailRepository,
            IShippingAddressRepository shippingAddressRepository,
            IOrderHistoryRepository orderHistoryRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _discountRepository = discountRepository;
            _cartService = cartService;
            _orderDetailRepository = orderDetailRepository;
            _shippingAddressRepository = shippingAddressRepository;
            _orderHistoryRepository = orderHistoryRepository;
        }

        public async Task<Order> CreateOrderAsync(Guid userId, Guid discountId, Guid shippingAddressId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) throw new AppException($"Người dùng không tồn tại.");

                var shippingAddress = await _shippingAddressRepository.GetByIdAsync(shippingAddressId);
                if (shippingAddress == null) throw new AppException($"Địa chỉ giao hàng không tồn tại.");

                var cartItems = await _cartService.GetCartDetailsAsync(userId);
                if (!cartItems.Any()) throw new AppException("Giỏ hàng trống.");

                double totalPrice = await _cartService.GetTotalPriceCartAsync(userId);

                if (discountId != Guid.Empty)
                {
                    var discount = await _discountRepository.GetByIdAsync(discountId);
                    if (discount == null) throw new AppException("Mã giảm giá không hợp lệ.");
                    if (discount.StartDate > DateTime.Now || discount.EndDate < DateTime.Now)
                        throw new AppException("Mã giảm giá đã hết hạn.");

                    totalPrice -= discount.Price;
                    if (totalPrice < 0) totalPrice = 0;
                }

                var newOrder = new Order
                {
                    OrderId = Guid.NewGuid(),
                    UserId = userId,
                    DiscountId = discountId != Guid.Empty ? discountId : Guid.Empty,
                    ShippingAddressId = shippingAddressId,
                    TotalPrice = totalPrice,
                    Status = 1 // Pending
                };

                await _orderRepository.CreateOrderAsync(newOrder);

                foreach (var cartItem in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderDetailId = Guid.NewGuid(),
                        OrderId = newOrder.OrderId,
                        BookId = cartItem.BookId,
                        Quantity = cartItem.Quantity
                    };

                    await _orderDetailRepository.AddAsync(orderDetail);
                }

                await _cartService.ClearCartAsync(userId);

                var orderHistory = new OrderHistory
                {
                    OrderHistoryId = Guid.NewGuid(),
                    OrderId = newOrder.OrderId,
                    Status = 1, // Pending
                    createdStatus = DateTime.Now
                };

                await _orderHistoryRepository.AddOrderHistoryAsync(orderHistory);
                await _orderRepository.SaveChangesAsync();

                return newOrder;
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi tạo đơn hàng: {ex.Message}");
            }
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.FindOrderByIdAsync(orderId);
                if (order == null) throw new AppException("Đơn hàng không tồn tại.");
                return order;
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi lấy thông tin đơn hàng: {ex.Message}");
            }
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            try
            {
                var orders = await _orderRepository.FindOrdersByUserIdAsync(userId);
                return orders;
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi lấy danh sách đơn hàng: {ex.Message}");
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            try
            {
                return await _orderRepository.GetAllOrdersAsync();
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi lấy tất cả đơn hàng: {ex.Message}");
            }
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(int status)
        {
            try
            {
                if (status < 1 || status > 5) throw new AppException("Trạng thái không hợp lệ.");
                return await _orderRepository.GetOrdersByStatusAsync(status);
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi lấy đơn hàng theo trạng thái: {ex.Message}");
            }
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, int status)
        {
            try
            {
                if (status < 1 || status > 5) throw new AppException("Trạng thái không hợp lệ.");

                var order = await _orderRepository.FindOrderByIdAsync(orderId);
                if (order == null) throw new AppException("Đơn hàng không tồn tại.");

                order.Status = status;
                await _orderRepository.UpdateOrderStatusAsync(orderId, status);

                var orderHistory = new OrderHistory
                {
                    OrderHistoryId = Guid.NewGuid(),
                    OrderId = orderId,
                    Status = status,
                    createdStatus = DateTime.Now
                };

                await _orderHistoryRepository.AddOrderHistoryAsync(orderHistory);
                await _orderRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi cập nhật trạng thái đơn hàng: {ex.Message}");
            }
        }

        public async Task UpdateShippingAddressAsync(Guid orderId, Guid shippingAddressId)
        {
            try
            {
                var order = await _orderRepository.FindOrderByIdAsync(orderId);
                if (order == null) throw new AppException("Đơn hàng không tồn tại.");

                var shippingAddress = await _shippingAddressRepository.GetByIdAsync(shippingAddressId);
                if (shippingAddress == null) throw new AppException("Địa chỉ giao hàng không tồn tại.");

                order.ShippingAddressId = shippingAddressId;
                await _orderRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi cập nhật địa chỉ giao hàng: {ex.Message}");
            }
        }

        public async Task CancelOrderAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.FindOrderByIdAsync(orderId);
                if (order == null) throw new AppException("Đơn hàng không tồn tại.");

                // Only pending orders can be canceled
                if (order.Status != 1) 
                    throw new AppException("Chỉ có thể huỷ đơn hàng khi ở trạng thái chờ xử lý.");

                await UpdateOrderStatusAsync(orderId, 3); // Canceled status
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi huỷ đơn hàng: {ex.Message}");
            }
        }

        public async Task<List<OrderDetail>> GetOrderDetailsAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.FindOrderByIdAsync(orderId);
                if (order == null) throw new AppException("Đơn hàng không tồn tại.");

                return await _orderDetailRepository.GetByOrderIdAsync(orderId);
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi lấy chi tiết đơn hàng: {ex.Message}");
            }
        }

        public async Task<List<OrderHistory>> GetOrderHistoryAsync(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.FindOrderByIdAsync(orderId);
                if (order == null) throw new AppException("Đơn hàng không tồn tại.");

                return await _orderHistoryRepository.GetOrderHistoryAsync(orderId);
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi lấy lịch sử đơn hàng: {ex.Message}");
            }
        }
    }
}