using System.Security.Claims;
using DocumentFormat.OpenXml.Drawing.Charts;
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
        private readonly IBookService _bookService;
        private readonly IOrderService _orderService;


        public OrderController(IUnitOfWork unitOfWork, ICartService cartService, IHttpContextAccessor httpContextAccessor, JwtUtils jwtUtils, IBookService bookService, IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _jwtUtils = jwtUtils;
            _bookService = bookService;
            _orderService = orderService;
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
    }

}