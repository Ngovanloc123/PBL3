using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using StackBook.DAL.IRepository;
using StackBook.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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
        private readonly IBookService _bookService;
        private readonly IDiscountService _discountService;

        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IDiscountRepository discountRepository,
            ICartService cartService,
            IOrderDetailRepository orderDetailRepository,
            IShippingAddressRepository shippingAddressRepository,
            IOrderHistoryRepository orderHistoryRepository,
            IBookService bookService,
            IUnitOfWork unitOfWork, IDiscountService discountService)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _discountRepository = discountRepository;
            _cartService = cartService;
            _orderDetailRepository = orderDetailRepository;
            _shippingAddressRepository = shippingAddressRepository;
            _orderHistoryRepository = orderHistoryRepository;
            _bookService = bookService;
            _unitOfWork = unitOfWork;
            _discountService = discountService;
        }

        public async Task<Order> CreateOrderAsync(Guid userId, CheckoutRequest request)
        {
            try
            {
                // Validate selected books
                if (request.SelectedBooks == null || !request.SelectedBooks.Any())
                {
                    throw new ArgumentException("No books selected for checkout.");
                }

                // Validate shipping address
                if (request.shippingAddressDefault?.ShippingAddressId == null)
                {
                    throw new ArgumentException("Shipping address is required.");
                }

                // Calculate total and validate books
                double totalAmount = 0;
                var orderDetails = new List<OrderDetail>();

                foreach (var selectedBook in request.SelectedBooks)
                {
                    var book = await _unitOfWork.Book.GetAsync(b => b.BookId == selectedBook.Book.BookId);
                    if (book == null)
                    {
                        throw new ArgumentException($"Book with ID {selectedBook.Book.BookId} not found.");
                    }

                    // Check stock availability
                    if (book.Stock < selectedBook.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for {book.BookTitle}. Available: {book.Stock}");
                    }

                    totalAmount += book.Price * selectedBook.Quantity;
                    orderDetails.Add(new OrderDetail
                    {
                        OrderDetailId = Guid.NewGuid(),
                        BookId = selectedBook.Book.BookId,
                        Quantity = selectedBook.Quantity
                    });
                }

                // Discount
                var discount = await _discountRepository.GetAsync(d => d.DiscountId == request.DiscountId);


                if (discount == null)
                // Lấy discount mặc định không giảm giá khi không chọn vào discount
                {
                    //discount = await _discountService.GetDiscountByCode("0");
                    //if (discount == null)
                    //{
                    discount = await _discountService.CreateDefaultDiscount();
                    //}
                }


                // Tính lại giá sau khi giảm giá
                totalAmount = Math.Max(0, totalAmount - discount.Price);


                // Create order
                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    UserId = userId,
                    DiscountId = discount.DiscountId,
                    ShippingAddressId = request.shippingAddressDefault.ShippingAddressId,
                    TotalPrice = totalAmount,
                    Status = 1,
                    CreatedAt = DateTime.Now,
                };

                // kiểm tra discount đã được dùng hay chưa
                var oldOrder = await _unitOfWork.Order.GetAsync(o => o.DiscountId == discount.DiscountId);

                if (oldOrder != null)
                {
                    throw new ApplicationException("Discount has been used");
                }
                //
                var shippingAddressId = request.shippingAddressDefault.ShippingAddressId;

                //var shippingAddress = await _unitOfWork.ShippingAddress.GetAsync(
                //    s => s.ShippingAddressId == shippingAddressId);

                //if (shippingAddress == null)
                //{
                //    throw new ArgumentException("Shipping address does not exist.");
                //}

                await _unitOfWork.Order.AddAsync(order);
                await _unitOfWork.SaveAsync();

                // Add order details
                foreach (var detail in orderDetails)
                {
                    detail.OrderId = order.OrderId;
                    await _unitOfWork.OrderDetail.AddAsync(detail);
                }

                // Update book quantities
                foreach (var selectedBook in request.SelectedBooks)
                {
                    await _bookService.UpdateBookQuantity(selectedBook.Book.BookId, selectedBook.Quantity, 1);
                }

                // Create order history 
                // var orderHistory = new OrderHistory
                // {
                //    OrderHistoryId = Guid.NewGuid(),
                //    OrderId = order.OrderId,
                //    Status = 1, // Pending
                //    createdStatus = DateTime.Now
                // };
                var orderHistory = await CreateOrderHistoryAsync(order.OrderId, 1); // status = 1. Pending

                //await _unitOfWork.OrderHistory.AddAsync(orderHistory);

                // Create payment record
                var payment = new Payment
                {
                    //PaymentId = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    PaymentMethod = request.PaymentMethod,
                    //PaymentMethod = "hahaha",
                    PaymentStatus = "0", // Pending
                    CreatedPayment = DateTime.Now
                };

                // Xóa sách khỏi giỏ hàng
                foreach (var selectedBook in request.SelectedBooks)
                {
                    await _cartService.RemoveFromCartAsync(userId, selectedBook.Book.BookId);
                }

                await _unitOfWork.Payment.AddAsync(payment);
                await _unitOfWork.SaveAsync();
                return order;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new ApplicationException($"{ex.Message}", ex);
            }
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, int status)
        {
            // Validate status (1-5)
            if (status < 1 || status > 5)
                throw new AppException("Trạng thái không hợp lệ.");

            // Get order or throw exception if not found
            var order = await _orderRepository.FindOrderByIdAsync(orderId) ??
                throw new AppException("Đơn hàng không tồn tại.");

            // If status is not changing, do nothing
            if (order.Status == status) return;

            // Process status update in transaction
            using (var transaction = await _orderRepository.BeginTransactionAsync())
            {
                try
                {
                    // Handle book quantity changes based on status transitions
                    if (status == 3) // Hủy đơn hàng
                    {
                        await RestoreBookQuantities(orderId); // Trả lại số lượng sách
                    }
                    else if (status == 5) // Trả hàng
                    {
                        await RestoreBookQuantities(orderId); // Trả lại số lượng sách
                    }
                    else if (order.Status == 4 && status != 4) // Từ hủy sang trạng thái khác (không phải hủy)
                    {
                        await DeductBookQuantities(orderId); // Trừ lại số lượng sách
                    }
                    else if (order.Status == 5 && status != 5) // Từ trả hàng sang trạng thái khác (không phải trả hàng)
                    {
                        await DeductBookQuantities(orderId); // Trừ lại số lượng sách
                    }

                    // Update order status
                    order.Status = status;
                    await _orderRepository.UpdateOrderStatusAsync(orderId, status);

                    // Add order history
                    await _orderHistoryRepository.AddOrderHistoryAsync(new OrderHistory
                    {
                        OrderHistoryId = Guid.NewGuid(),
                        OrderId = orderId,
                        Status = status,
                        createdStatus = DateTime.Now
                    });

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new AppException($"Lỗi khi cập nhật trạng thái đơn hàng: {ex.Message}");
                }
            }
        }

        public async Task CancelOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.FindOrderByIdAsync(orderId) ??
                throw new AppException("Đơn hàng không tồn tại.");

            if (order.Status != 1)
                throw new AppException("Chỉ có thể huỷ đơn hàng khi ở trạng thái chờ xử lý.");

            await UpdateOrderStatusAsync(orderId, 3)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        ; // Canceled status
        }

        private async Task RestoreBookQuantities(Guid orderId)
        {
            var orderDetails = await _orderDetailRepository.GetByOrderIdAsync(orderId);
            foreach (var detail in orderDetails)
            {
                var book = await _bookService.GetByIdAsync(detail.BookId);
                if (book != null)
                {
                    book.Stock += detail.Quantity;
                    await _bookService.UpdateAsync(book);
                }
            }
        }

        private async Task DeductBookQuantities(Guid orderId)
        {
            var orderDetails = await _orderDetailRepository.GetByOrderIdAsync(orderId);
            foreach (var detail in orderDetails)
            {
                var book = await _bookService.GetByIdAsync(detail.BookId);
                if (book != null)
                {
                    if (book.Stock < detail.Quantity)
                        throw new AppException($"Số lượng sách '{book.BookTitle}' không đủ. Chỉ còn {book.Stock} cuốn.");

                    book.Stock -= detail.Quantity;
                    await _bookService.UpdateAsync(book);
                }
            }
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _unitOfWork.Order.GetAsync(o => o.OrderId == orderId, "OrderDetails.Book.Authors,User,ShippingAddress,Discount");
            if (order == null)
                throw new AppException("Order does not exist.");
            return order;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            var orders = await _unitOfWork.Order.GetListAsync(o => o.UserId == userId, "OrderDetails.Book,ShippingAddress,User,Payments");
            return orders.ToList().OrderByDescending(o => o.CreatedAt).ToList() ?? new List<Order>(); ;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Order.GetAllAsync("OrderDetails.Book,ShippingAddress,User");
            return orders.ToList().OrderByDescending(o => o.Status).ToList();

        }

        public async Task<List<Order>> GetOrdersByStatusAsync(int status)
        {
            if (status < 1 || status > 5)
                throw new AppException("Trạng thái không hợp lệ.");

            var orders = await _unitOfWork.Order.GetListAsync(o => o.Status == status, "OrderDetails.Book,ShippingAddress,User");

            return orders.ToList().OrderByDescending(o => o.CreatedAt).ToList() ?? new List<Order>(); ;
        }

        public async Task<List<Order>> GetOrdersByUserIdAndStatusAsync(Guid userId, int status)
        {
            if (status < 1 || status > 5)
                throw new AppException("Trạng thái không hợp lệ.");

            var orders = await _unitOfWork.Order.GetListAsync(o => o.Status == status && o.UserId == userId, "OrderDetails.Book,ShippingAddress");

            return orders.ToList().OrderByDescending(o => o.CreatedAt).ToList() ?? new List<Order>(); ;
        }

        public async Task UpdateShippingAddressAsync(Guid orderId, Guid shippingAddressId)
        {
            var order = await _orderRepository.FindOrderByIdAsync(orderId) ??
                throw new AppException("Đơn hàng không tồn tại.");

            var shippingAddress = await _shippingAddressRepository.GetByIdAsync(shippingAddressId) ??
                throw new AppException("Địa chỉ giao hàng không tồn tại.");

            order.ShippingAddressId = shippingAddressId;
            await _orderRepository.SaveChangesAsync();
        }

        public async Task<List<OrderDetail>> GetOrderDetailsAsync(Guid orderId)
        {
            _ = await _orderRepository.FindOrderByIdAsync(orderId) ??
                throw new AppException("Đơn hàng không tồn tại.");

            return await _orderDetailRepository.GetByOrderIdAsync(orderId) ??
                new List<OrderDetail>();
        }

        public async Task<List<OrderHistory>> GetOrderHistoryAsync(Guid orderId)
        {
            var order = await _orderRepository.FindOrderByIdAsync(orderId) ??
                throw new AppException("Đơn hàng không tồn tại.");

            return await _orderHistoryRepository.GetOrderHistoryAsync(orderId) ??
                new List<OrderHistory>();
        }

        public async Task<OrderHistory> CreateOrderHistoryAsync(Guid orderId, int status)
        {
            // Create order history 
            var orderHistory = new OrderHistory
            {
                OrderHistoryId = Guid.NewGuid(),
                OrderId = orderId,
                Status = status, // Pending
                createdStatus = DateTime.Now
            };

            await _unitOfWork.OrderHistory.AddAsync(orderHistory);
            await _unitOfWork.SaveAsync();
            return orderHistory;
        }
        //lay order tu discountId
        public async Task<Order?> GetOrderByDiscountIdAsync(Guid discountId)
        {
            var order = await _orderRepository.GetOrderByDiscountId(discountId);
            return order;
        }
    }
}