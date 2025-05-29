using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Utils;
using StackBook.ViewModels;

namespace StackBook.Areas.Customer.Controllers
{

    [Area("Customer")]
    [Authorize]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtUtils _jwtUtils;

        public OrderController(IUnitOfWork unitOfWork, ICartService cartService, IHttpContextAccessor httpContextAccessor, JwtUtils jwtUtils)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _jwtUtils = jwtUtils;
        }

        //[Authorize(Roles = "Customer")]
        //[Area("Customer")]
        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult BuyNow(Guid bookId)
        {
            // Logic xử lý mua ngay
            TempData["Message"] = "Proceeding to checkout!";
            return RedirectToAction("Checkout", "Order");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutRequest request, String PaymentMethod)
        {
            try
            {
                // Validate shipping address
                if (request.shippingAddressDefault?.ShippingAddressId == null)
                {
                    TempData["Error"] = "Please select a shipping address.";
                    return RedirectToAction("Checkout", "Cart");
                }

                // Validate selected books
                if (request.SelectedBooks == null || !request.SelectedBooks.Any())
                {
                    TempData["Error"] = "No books selected for checkout.";
                    return RedirectToAction("Checkout", "Cart");
                }

                // Get current user
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return NotFound("Invalid user.");
                }

                var user = await _unitOfWork.User.GetAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Validate shipping address exists
                var shippingAddress = await _unitOfWork.ShippingAddress.GetAsync(
                    a => a.ShippingAddressId == request.shippingAddressDefault.ShippingAddressId && a.UserId == userId);

                if (shippingAddress == null)
                {
                    TempData["Error"] = "Invalid shipping address.";
                    return RedirectToAction("Checkout", "Cart");
                }

                // Calculate total and validate books
                double totalAmount = 0;
                var orderDetails = new List<OrderDetail>();

                foreach (var selectedBook in request.SelectedBooks)
                {
                    var book = await _unitOfWork.Book.GetAsync(b => b.BookId == selectedBook.Book.BookId);
                    if (book == null)
                    {
                        TempData["Error"] = $"Book with ID {selectedBook.Book.BookId} not found.";
                        return RedirectToAction("Checkout", "Cart");
                    }

                    // Check stock availability
                    if (book.Stock < selectedBook.Quantity)
                    {
                        TempData["Error"] = $"Insufficient stock for {book.BookTitle}. Available: {book.Stock}";
                        return RedirectToAction("Checkout", "Cart");
                    }

                    totalAmount += book.Price * selectedBook.Quantity;
                    orderDetails.Add(new OrderDetail
                    {
                        BookId = selectedBook.Book.BookId,
                        Quantity = selectedBook.Quantity
                    });
                }

                //// Get default discount (you might want to handle discount selection logic here)
                //var defaultDiscount = await _unitOfWork.DiscountRepository.Get(d => d.IsDefault == true);
                //if (defaultDiscount == null)
                //{
                //    // Create a no discount entry or handle as needed
                //    defaultDiscount = new Discount
                //    {
                //        DiscountId = Guid.NewGuid(),
                //        DiscountValue = 0,
                //        DiscountName = "No Discount"
                //    };
                //}

                
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
                

                // Create order
                var order = new Order
                {
                    UserId = userId,
                    DiscountId = defaultDiscount.DiscountId, // làm lại nếu discount không có
                    ShippingAddressId = request.shippingAddressDefault.ShippingAddressId,
                    TotalPrice = totalAmount,
                    Status = 1 // Pending
                };

                _unitOfWork.Order.AddAsync(order);
                await _unitOfWork.SaveAsync();

                // Add order details
                foreach (var detail in orderDetails)
                {
                    detail.OrderId = order.OrderId;
                    _unitOfWork.OrderDetail.AddAsync(detail);
                }

                // Update book quantities
                foreach (var selectedBook in request.SelectedBooks)
                {
                    var book = await _unitOfWork.Book.GetAsync(b => b.BookId == selectedBook.Book.BookId);
                    if (book != null)
                    {
                        book.Stock -= selectedBook.Quantity;
                        _unitOfWork.Book.UpdateAsync(book);
                    }
                }

                //// Create order history entry
                //var orderHistory = new OrderHistory
                //{
                //    OrderHistoryId = Guid.NewGuid(),

                //    OrderId = order.OrderId,
                //    Status = 1, // Pending
                //    createdStatus = DateTime.Now
                //};



                //_unitOfWork.OrderHistory.AddAsync(orderHistory);
                //await _unitOfWork.SaveAsync();

                // Tạo payment
                var payment = new Payment
                {
                    OrderId = order.OrderId,
                    PaymentMethod = PaymentMethod,
                    PaymentStatus = "0",
                    CreatedPayment = DateTime.Now
                };

                _unitOfWork.Payment.AddAsync(payment);
                await _unitOfWork.SaveAsync();

                // Clear cart from session if exists
                //HttpContext.Session.Remove("ShoppingCart");

                TempData["Success"] = "Order placed successfully! You will pay when receiving your order.";
                return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
            }
            catch (DbUpdateException ex)
            {
                // Log inner exception để biết chi tiết
                var innerEx = ex.InnerException;
                TempData["Error"] = $"Database error: {innerEx?.Message}";
                return RedirectToAction("Checkout", "Cart");
            }
            catch (Exception ex)
            {
                // Log exception here
                TempData["Error"] = "An error occurred while placing your order. Please try again.";
                return RedirectToAction("Checkout", "Cart");
            }
        }

        [HttpGet]
        [Route("Customer/Order/OrderConfirmation/{id}")]    
        public async Task<IActionResult> OrderConfirmation(Guid id)
        {
            // Sử dụng id để lấy thông tin order
            var order = await _unitOfWork.Order.GetAsync(o => o.OrderId == id, "OrderDetails,User,ShippingAddress");

            //if (order == null)
            //{
            //    return NotFound();
            //}

            return View(order);
        }
    }

}