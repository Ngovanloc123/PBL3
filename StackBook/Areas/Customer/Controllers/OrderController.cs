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



        public OrderController(IUnitOfWork unitOfWork, ICartService cartService, IHttpContextAccessor httpContextAccessor, JwtUtils jwtUtils, IBookService bookService, IOrderService orderService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _jwtUtils = jwtUtils;
            _bookService = bookService;
            _orderService = orderService;
            _userService = userService;
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
            ViewBag.User = user.Result.Data;
            ViewBag.Status = statusInt;


            return View(orders);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Cancel(Guid orderId)
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

                TempData["Success"] = "Order canceled successfully.";
                return RedirectToAction("Index", "Order");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Order");
            }
        }


    }

}