using System.Security.Claims;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackBook.DAL.IRepository;
using StackBook.Exceptions;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Services;
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
        private readonly IBookService _bookService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;

        public OrderController(IUnitOfWork unitOfWork, ICartService cartService, IHttpContextAccessor httpContextAccessor, JwtUtils jwtUtils, IBookService bookService, IOrderService orderService, IUserService userService, IReviewService reviewService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _jwtUtils = jwtUtils;
            _bookService = bookService;
            _orderService = orderService;
            _userService = userService;
            _reviewService = reviewService;
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
        public async Task<IActionResult> PlaceOrder(CheckoutRequest request)
        {
            try
            {
                // Get current user
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return NotFound("Invalid user.");
                }
                // tạo order và các bảng liên quan
                var orderResult = await _orderService.CreateOrderAsync(userId, request);

                // Xóa CheckoutRequest trên Session
                if (HttpContext.Session.Keys.Contains("CheckoutRequest"))
                {
                    HttpContext.Session.Remove("CheckoutRequest");
                }

                TempData["Success"] = "Order placed successfully! You will pay when receiving your order.";
                return RedirectToAction("OrderConfirmation", new { id = orderResult.OrderId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Checkout", "Cart");
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index", "Home");
            }

            // Verify user owns this order
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out Guid userId) && order.UserId != userId)
            {
                return Forbid();
            }

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return NotFound("Invalid user.");
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            // Trả về danh sách đơn hàng
            return View(orders);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index(string status)
        {

            var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });
            //Xem cookie access token có tồn tại hay không
            Console.WriteLine($"AccessToken: {_httpContextAccessor.HttpContext?.Request.Cookies["accessToken"]}");
            //Xem cookie refresh token có tồn tại hay không
            Console.WriteLine($"RefreshToken: {_httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"]}");
            //Sem user qua cookie
            var user = _userService.GetUserById(Guid.Parse(userIdValue));
            if (user == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User not found.", StatusCode = 404 });
            if (user.Result.Data == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User not found.", StatusCode = 404 });


            int statusInt = Convert.ToInt32(status);
            // Lây thông tin order
            var orders = new List<Models.Order>();
            if (statusInt == 0)
                orders = await _orderService.GetOrdersByUserIdAsync(Guid.Parse(userIdValue));
            else
            {
                orders = await _orderService.GetOrdersByUserIdAndStatusAsync(Guid.Parse(userIdValue), statusInt);
            }
                        //Lấy danh sách đơn hàng có status = 4 và trong đó các orderDetails có đánh giá hay chưa
            Dictionary<Guid, Dictionary<Guid, bool>> orderReviews = new Dictionary<Guid, Dictionary<Guid, bool>>();
            foreach (var order in orders)
            {
                if (order.Status == 4) // Chỉ lấy đơn hàng đã giao
                {
                    orderReviews[order.OrderId] = new Dictionary<Guid, bool>();
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        // Kiểm tra xem người dùng đã đánh giá cuốn sách này trong đơn hàng này chưa
                        var hasReviewed = await _reviewService.GetReviewByUserIdBookIdOrderIdAsync(Guid.Parse(userIdValue), orderDetail.BookId, order.OrderId);
                        orderReviews[order.OrderId][orderDetail.BookId] = hasReviewed;
                    }
                }
            }
            //in ra orderReviews để debug
            Console.WriteLine("Order Reviews:");
            foreach (var order in orderReviews)
            {
                Console.WriteLine($"Order ID: {order.Key}");
                foreach (var bookReview in order.Value)
                {
                    Console.WriteLine($"  Book ID: {bookReview.Key}, Reviewed: {bookReview.Value}");
                }
            }
            //lưu vào ViewData
            ViewData["OrderReviews"] = orderReviews;
            ViewBag.User = user.Result.Data;
            ViewBag.Status = statusInt;


            return View(orders);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Cancel(Guid orderId, int status)
        {
            try
            {
                var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdValue == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });

                // Check if access token exists in cookies  
                Console.WriteLine($"AccessToken: {_httpContextAccessor.HttpContext?.Request.Cookies["accessToken"]}");
                Console.WriteLine($"RefreshToken: {_httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"]}");

                // Retrieve user from cookie  
                var user = _userService.GetUserById(Guid.Parse(userIdValue));
                if (user == null || user.Result.Data == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "User not found.", StatusCode = 404 });

                // Cancel the order  
                await _orderService.CancelOrderAsync(orderId);
                // Tạo History
                await _orderService.CreateOrderHistoryAsync(orderId, 3);


                TempData["Success"] = "Order canceled successfully.";
                return RedirectToAction("Index", new { status = status });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", new { status = status });
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Received(Guid orderId, int status)
        {
            try
            {
                var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdValue == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });

                // Check if access token exists in cookies  
                Console.WriteLine($"AccessToken: {_httpContextAccessor.HttpContext?.Request.Cookies["accessToken"]}");
                Console.WriteLine($"RefreshToken: {_httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"]}");

                // Retrieve user from cookie  
                var user = _userService.GetUserById(Guid.Parse(userIdValue));
                if (user == null || user.Result.Data == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "User not found.", StatusCode = 404 });

                // Shipped -> Delivered
                await _orderService.UpdateOrderStatusAsync(orderId, 4);
                // Tạo History
                await _orderService.CreateOrderHistoryAsync(orderId, 4);

                TempData["Success"] = "Order canceled successfully.";
                return RedirectToAction("Index", new { status = status });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", new { status = status });
            }
        }


        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Return(Guid orderId, int status)
        {
            try
            {
                var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdValue == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });

                // Check if access token exists in cookies  
                Console.WriteLine($"AccessToken: {_httpContextAccessor.HttpContext?.Request.Cookies["accessToken"]}");
                Console.WriteLine($"RefreshToken: {_httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"]}");

                // Retrieve user from cookie  
                var user = _userService.GetUserById(Guid.Parse(userIdValue));
                if (user == null || user.Result.Data == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "User not found.", StatusCode = 404 });

                // Delivered -> Return
                await _orderService.UpdateOrderStatusAsync(orderId, 5);
                // Tạo History
                await _orderService.CreateOrderHistoryAsync(orderId, 5);

                TempData["Success"] = "Order canceled successfully.";
                return RedirectToAction("Index", new { status = status });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", new { status = status });
            }
        }

        [HttpPost("SubmitReview")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SubmitReview([FromForm] Guid orderId, [FromForm] Guid bookId, [FromForm] int rating, [FromForm] string comment)
        {
            try
            {
                // Get current user ID
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"UserId: {userIdString}");
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized("Invalid user.");
                }
                //kiểm tra dữ liệu đầu vào
                Console.WriteLine($"OrderId: {orderId}, BookId: {bookId}, Rating: {rating}, Comment: {comment}");
                if (orderId == Guid.Empty || bookId == Guid.Empty || rating < 1 || rating > 5)
                {
                    Console.WriteLine("Invalid input data for review submission.");
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid input data.", StatusCode = 400 });
                }
                //kiểm tra xem người dùng đã đánh giá cuốn sách này trong đơn hàng này chưa
                Console.WriteLine($"Checking for existing review for UserId: {userId}, BookId: {bookId}, OrderId: {orderId}");
                var existingReview = await _reviewService.GetReviewByUserIdBookIdOrderIdAsync(userId, bookId, orderId);
                if (!existingReview)
                { 
                    Console.WriteLine($"Existing Review found for UserId: {userId}, BookId: {bookId}, OrderId: {orderId}");
                    return View("Error", new ErrorViewModel { ErrorMessage = "Existing Review for book in order", StatusCode = 404 });
                }
                // Validate review
                if (!ModelState.IsValid)
                {
                    //trả về trang lỗi Error nếu model không hợp lệ
                    return View("Error", new ErrorViewModel { ErrorMessage = "Data in valid", StatusCode = 404 });
                }
                // Save review
                var createdReview = await _reviewService.CreateReviewFromOrderAsync(orderId, bookId, userId, rating, comment);
                if (createdReview == null)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Created Review unsucessfull", StatusCode = 404 });
                }
                // Return the created review
                return RedirectToAction("Index", "Order", new { status = 4 }); // Redirect to delivered orders
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting review: {ex.Message}");
                return View("Error", new ErrorViewModel { ErrorMessage = ex.Message, StatusCode = 500 });
            }
        }
    }
}