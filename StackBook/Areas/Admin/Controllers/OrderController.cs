using System.Security.Claims;
using AspNetCoreGeneratedDocument;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Services;
using StackBook.ViewModels;
using X.PagedList.Extensions;

namespace StackBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, IUnitOfWork unitOfWork, INotificationService notificationService, IUserService userService)
        {
            _orderService = orderService;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index(int? page, int status)
        {
            try
            {
                int pageSize = 8;
                int pageNumber = page ?? 1;
                var orders = new List<Models.Order>();
                if (status == 0)
                {
                    orders = await _orderService.GetAllOrdersAsync();
                }
                else
                {
                    orders = await _orderService.GetOrdersByStatusAsync(status);
                }

                ViewBag.Status = status.ToString();

                Dictionary<Guid, string> paymentMethods = new Dictionary<Guid, string>();
                foreach (var order in orders)
                {
                    // Get the first payment for the order (if any)
                    var payments = await _unitOfWork.Payment.GetByOrderIdAsync(order.OrderId);
                    var paymentMethod = payments.FirstOrDefault()?.PaymentMethod ?? "COD";
                    paymentMethods[order.OrderId] = paymentMethod;
                }
                
                ViewData["PaymentMethods"] = paymentMethods;
                var pagedOrders = orders.ToPagedList(pageNumber, pageSize);
                return View(pagedOrders);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error loading order: {ex.Message}";
                return View("Error", new { message = "Error loading order", error = ex.Message });
            }
        }
        [HttpGet]

        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return View(order);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error get order: {ex.Message}";
                return View("Error", new { message = "Error loading order", error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Cancel(Guid orderId)
        {
            try
            {

                // Cancel the order  
                await _orderService.CancelOrderAsync(orderId);
                await _orderService.CreateOrderHistoryAsync(orderId, 3);
                TempData["Success"] = "Order canceled successfully.";
                var order = await _orderService.GetOrderByIdAsync(orderId);
                // Create a notification for the user
                await _notificationService.SendNotificationAsync(order.UserId, "Order Canceled" + $"Your order with ID {orderId} has been canceled.");
                return RedirectToAction("Index", "Order");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Order");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(Guid orderId)
        {
            try
            {

                // Pending  -> Delivering
                await _orderService.UpdateOrderStatusAsync(orderId, 2);
                TempData["Success"] = "Order Confirmation Successfully.";
                var order = await _orderService.GetOrderByIdAsync(orderId);
                // Create a notification for the user
                await _notificationService.SendNotificationAsync(order.UserId, "Order Confirmed" + $"Your order with ID {orderId} has been confirmed and is now being processed.");
                return RedirectToAction("Detail", "Order", new { id = orderId });

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Order");
            }
        }

    }
}
