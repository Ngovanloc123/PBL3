// using StackBook.Interfaces;
// using StackBook.Models;
// using StackBook.Data;
// using Microsoft.EntityFrameworkCore;
// using StackBook.Exceptions;
// using StackBook.DAL.IRepository;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using System.Linq;

// namespace StackBook.Services
// {
//     public class OrderService : IOrderService
//     {
//         private readonly IOrderRepository _orderRepository;
//         private readonly IUserRepository _userRepository;
//         private readonly IDiscountRepository _discountRepository;
//         private readonly ICartService _cartService;
//         private readonly IOrderDetailRepository _orderDetailRepository;
//         private readonly IShippingAddressRepository _shippingAddressRepository;
//         private readonly IOrderHistoryRepository _orderHistoryRepository;
//         private readonly IBookService _bookService;

//         public OrderService(
//             IOrderRepository orderRepository,
//             IUserRepository userRepository,
//             IDiscountRepository discountRepository,
//             ICartService cartService,
//             IOrderDetailRepository orderDetailRepository,
//             IShippingAddressRepository shippingAddressRepository,
//             IOrderHistoryRepository orderHistoryRepository,
//             IBookService bookService)
//         {
//             _orderRepository = orderRepository;
//             _userRepository = userRepository;
//             _discountRepository = discountRepository;
//             _cartService = cartService;
//             _orderDetailRepository = orderDetailRepository;
//             _shippingAddressRepository = shippingAddressRepository;
//             _orderHistoryRepository = orderHistoryRepository;
//             _bookService = bookService;
//         }

//         public async Task<Order> CreateOrderAsync(Guid userId, Guid discountId, Guid shippingAddressId)
//         {
//             try
//             {
//                 var user = await _userRepository.GetByIdAsync(userId);
//                 if (user == null) throw new AppException($"Người dùng không tồn tại.");

//                 var shippingAddress = await _shippingAddressRepository.GetByIdAsync(shippingAddressId);
//                 if (shippingAddress == null) throw new AppException($"Địa chỉ giao hàng không tồn tại.");

//                 var cart = await _cartService.GetCartDetailsAsync(userId);
//                 if (cart == null) throw new AppException("Giỏ hàng trống.");

//                 double totalPrice = await _cartService.GetTotalPriceCartAsync(userId);

//                 if (discountId != Guid.Empty)
//                 {
//                     var discount = await _discountRepository.GetByIdAsync(discountId);
//                     if (discount == null) throw new AppException("Mã giảm giá không hợp lệ.");
//                     if (discount.StartDate > DateTime.Now || discount.EndDate < DateTime.Now)
//                         throw new AppException("Mã giảm giá đã hết hạn.");

//                     totalPrice -= discount.Price;
//                     if (totalPrice < 0) totalPrice = 0;
//                 }

//                 var newOrder = new Order
//                 {
//                     OrderId = Guid.NewGuid(),
//                     UserId = userId,
//                     DiscountId = discountId != Guid.Empty ? discountId : Guid.Empty,
//                     ShippingAddressId = shippingAddressId,
//                     TotalPrice = totalPrice,
//                     Status = 1 // Pending
//                 };

//                 await _orderRepository.CreateOrderAsync(newOrder);

//                 foreach (var cartItem in cart.CartDetails)
//                 {
//                     var orderDetail = new OrderDetail
//                     {
//                         OrderDetailId = Guid.NewGuid(),
//                         OrderId = newOrder.OrderId,
//                         BookId = cartItem.BookId,
//                         Quantity = cartItem.Quantity
//                     };

//                     await _orderDetailRepository.AddAsync(orderDetail);
//                 }

//                 await _cartService.ClearCartAsync(userId);

//                 var orderHistory = new OrderHistory
//                 {
//                     OrderHistoryId = Guid.NewGuid(),
//                     OrderId = newOrder.OrderId,
//                     Status = 1, // Pending
//                     createdStatus = DateTime.Now
//                 };

//                 await _orderHistoryRepository.AddOrderHistoryAsync(orderHistory);
//                 await _orderRepository.SaveChangesAsync();

//                 return newOrder;
//             }
//             catch (Exception ex)
//             {
//                 throw new AppException($"Lỗi khi tạo đơn hàng: {ex.Message}");
//             }
//         }

//         public async Task<Order> GetOrderByIdAsync(Guid orderId)
//         {
//             try
//             {
//                 var order = await _orderRepository.FindOrderByIdAsync(orderId);
//                 if (order == null) throw new AppException("Đơn hàng không tồn tại.");
//                 return order;
//             }
//             catch (Exception ex)
//             {
//                 throw new AppException($"Lỗi khi lấy thông tin đơn hàng: {ex.Message}");
//             }
//         }

//         public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
//         {
//             try
//             {
//                 var orders = await _orderRepository.FindOrdersByUserIdAsync(userId);
//                 return orders;
//             }
//             catch (Exception ex)
//             {
//                 throw new AppException($"Lỗi khi lấy danh sách đơn hàng: {ex.Message}");
//             }
//         }

//         public async Task<List<Order>> GetAllOrdersAsync()
//         {
//             try
//             {
//                 return await _orderRepository.GetAllOrdersAsync();
//             }
//             catch (Exception ex)
//             {
//                 throw new AppException($"Lỗi khi lấy tất cả đơn hàng: {ex.Message}");
//             }
//         }

//         public async Task<List<Order>> GetOrdersByStatusAsync(int status)
//         {
//             try
//             {
//                 if (status < 1 || status > 5) throw new AppException("Trạng thái không hợp lệ.");
//                 return await _orderRepository.GetOrdersByStatusAsync(status);
//             }
//             catch (Exception ex)
//             {
//                 throw new AppException($"Lỗi khi lấy đơn hàng theo trạng thái: {ex.Message}");
//             }
//         }

//         public async Task UpdateOrderStatusAsync(Guid orderId, int status)
//         {
//             try
//             {
//                 if (status < 1 || status > 5) throw new AppException("Trạng thái không hợp lệ.");

//                 var order = await _orderRepository.FindOrderByIdAsync(orderId);
//                 if (order == null) throw new AppException("Đơn hàng không tồn tại.");

//                 order.Status = status;
//                 await _orderRepository.UpdateOrderStatusAsync(orderId, status);

//                 var orderHistory = new OrderHistory
//                 {
//                     OrderHistoryId = Guid.NewGuid(),
//                     OrderId = orderId,
//                     Status = status,
//                     createdStatus = DateTime.Now
//                 };

//                 await _orderHistoryRepository.AddOrderHistoryAsync(orderHistory);
//                 await _orderRepository.SaveChangesAsync();
//             }
//             catch (Exception ex)
//             {
//                 throw new AppException($"Lỗi khi cập nhật trạng thái đơn hàng: {ex.Message}");
//             }
//         }

//         public async Task UpdateShippingAddressAsync(Guid orderId, Guid shippingAddressId)
//         {
//             try
//             {
//                 var order = await _orderRepository.FindOrderByIdAsync(orderId);
//                 if (order == null) throw new AppException("Đơn hàng không tồn tại.");

//                 var shippingAddress = await _shippingAddressRepository.GetByIdAsync(shippingAddressId);
//                 if (shippingAddress == null) throw new AppException("Địa chỉ giao hàng không tồn tại.");

//                 order.ShippingAddressId = shippingAddressId;
//                 await _orderRepository.SaveChangesAsync();
//             }
//             catch (Exception ex)
//             {
//                 throw new AppException($"Lỗi khi cập nhật địa chỉ giao hàng: {ex.Message}");
//             }
//         }

//         public async Task CancelOrderAsync(Guid orderId)
//         {
//             try
//             {
//                 var order = await _orderRepository.FindOrderByIdAsync(orderId);
//                 if (order == null) throw new AppException("Đơn hàng không tồn tại.");

//                 // Only pending orders can be canceled
//                 if (order.Status != 1) 
//                     throw new AppException("Chỉ có thể huỷ đơn hàng khi ở trạng thái chờ xử lý.");

//                 await UpdateOrderStatusAsync(orderId, 3); // Canceled status
//             }
//             catch (Exception ex)
//             {
//                 throw new AppException($"Lỗi khi huỷ đơn hàng: {ex.Message}");
//             }
//         }

//         public async Task<List<OrderDetail>> GetOrderDetailsAsync(Guid orderId)
//         {
//             try
//             {
//                 var order = await _orderRepository.FindOrderByIdAsync(orderId);
//                 if (order == null) throw new AppException("Đơn hàng không tồn tại.");

//                 return await _orderDetailRepository.GetByOrderIdAsync(orderId);
//             }
//             catch (Exception ex)
//             {
//                 throw new AppException($"Lỗi khi lấy chi tiết đơn hàng: {ex.Message}");
//             }
//         }

//         public async Task<List<OrderHistory>> GetOrderHistoryAsync(Guid orderId)
//         {
//             try
//             {
//                 var order = await _orderRepository.FindOrderByIdAsync(orderId);
//                 if (order == null) throw new AppException("Đơn hàng không tồn tại.");

//                 return await _orderHistoryRepository.GetOrderHistoryAsync(orderId);
//             }
//             catch (Exception ex)
//             {
//                 throw new AppException($"Lỗi khi lấy lịch sử đơn hàng: {ex.Message}");
//             }
//         }
//     }
// }
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
            IUnitOfWork unitOfWork)
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



                // Đưa qua service Discount

                // Xét lại thêm vào view
                var defaultDiscount = new Discount
                {
                    DiscountName = "Test Discount",
                    Price = 1.99,
                    DiscountCode = "102230197",
                    Description = "Mã giảm giá test",
                    CreatedDiscount = DateTime.Now,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,

                };
                await _unitOfWork.Discount.AddAsync(defaultDiscount);
                await _unitOfWork.SaveAsync();

                Discount? discount = null;
                if (!string.IsNullOrEmpty(defaultDiscount.DiscountName))
                {
                    discount = await _unitOfWork.Discount.GetAsync(d => d.DiscountId == defaultDiscount.DiscountId);
                    if (discount != null)
                    {
                        // Validate discount dates
                        if (discount.StartDate > DateTime.Now || discount.EndDate < DateTime.Now)
                        {
                            // Vì discount test nên thời gian hơi lỏ
                            //throw new InvalidOperationException("Discount code has expired.");
                        }
                        //totalAmount = Math.Max(0, totalAmount - discount.Price);
                    }
                }





               // Create order
                var order = new Order
                {
                    UserId = userId,
                    DiscountId = defaultDiscount!.DiscountId,
                    ShippingAddressId = request.shippingAddressDefault.ShippingAddressId,
                    TotalPrice = totalAmount,
                    Status = 1, // Pending
                                //CreatedDate = DateTime.Now
                };

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
                var orderHistory = new OrderHistory
                {
                    OrderHistoryId = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    Status = 1, // Pending
                    createdStatus = DateTime.Now
                };

                await _unitOfWork.OrderHistory.AddAsync(orderHistory);

                // Create payment record
                var payment = new Payment
                {
                    PaymentId = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    PaymentMethod = request.PaymentMethod,
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
                throw new ApplicationException($"Error creating order: {ex.Message}", ex);
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
                    if (status == 4) // Hủy đơn hàng
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

            await UpdateOrderStatusAsync(orderId, 3); // Canceled status
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
            var order = await _unitOfWork.Order.GetAsync(o => o.OrderId == orderId, "OrderDetails.Book.Authors,User,ShippingAddress");
            if (order == null) 
                throw new AppException("Order does not exist."); 
            return order;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _orderRepository.FindOrdersByUserIdAsync(userId) ?? 
                new List<Order>();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync() ?? 
                new List<Order>();
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(int status)
        {
            if (status < 1 || status > 5) 
                throw new AppException("Trạng thái không hợp lệ.");
            
            return await _orderRepository.GetOrdersByStatusAsync(status) ?? 
                new List<Order>();
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
    }
}