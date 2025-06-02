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
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
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
