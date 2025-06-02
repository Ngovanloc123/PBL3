using Microsoft.AspNetCore.Mvc;
using StackBook.Interfaces;
using StackBook.Models;
using System;
using System.Threading.Tasks;

namespace StackBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("Customer/[controller]")]
    public class DiscountsController : Controller
    {
        private readonly IDiscountService _discountService;

        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        // Xem tất cả mã giảm giá
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả mã giảm giá từ dịch vụ check ngày hệ thống
            var discounts = await _discountService.GetAllDiscounts();
            // if (discounts == null || discounts.Count == 0)
            // {
            //     TempData["ErrorMessage"] = "No active discounts available.";
            //     return RedirectToAction("Index", "Home");
            // }
            return View(discounts);
        }
    }
}