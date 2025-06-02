using Microsoft.AspNetCore.Mvc;
using StackBook.Interfaces;
using StackBook.ViewModels;
using StackBook.Models;

namespace YourProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiscountController : Controller
    {
        private readonly IDiscountService _discountService;
        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }
        [HttpGet("Discount/Index")]
        public async Task<IActionResult> Index()
        {
            var discounts = await _discountService.GetAllDiscounts();
            return View(discounts);
        }
        [HttpGet("Discount/Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost("Discount/Create")]
        public async Task<IActionResult> Create(Discount discount)
        {
            if (discount == null)
            {
                return BadRequest("Discount cannot be null");
            }
            if (string.IsNullOrEmpty(discount.DiscountCode))
            {
                ModelState.AddModelError("DiscountCode", "Discount code is required.");
            }
            if (discount.StartDate >= discount.EndDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
            }
            if (!ModelState.IsValid)
            {
                return View(discount);
            }
            await _discountService.CreateDiscount(discount);
            return RedirectToAction("Index");
        }
        [HttpGet("Discount/Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var discount = await _discountService.GetDiscountByCode(id.ToString());
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }
        [HttpPost("Discount/Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, Discount discount)
        {
            if (id != discount.DiscountId)
            {
                return BadRequest("Discount ID mismatch.");
            }
            if (string.IsNullOrEmpty(discount.DiscountCode))
            {
                ModelState.AddModelError("DiscountCode", "Discount code is required.");
            }
            if (discount.StartDate >= discount.EndDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
            }
            if (!ModelState.IsValid)
            {
                return View(discount);
            }
            await _discountService.UpdateDiscount(discount);
            return RedirectToAction("Index");
        }
        [HttpGet("Discount/Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var discount = await _discountService.GetDiscountByCode(id.ToString());
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }
        [HttpPost("Discount/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var discount = await _discountService.GetDiscountByCode(id.ToString());
            if (discount == null)
            {
                return NotFound();
            }
            await _discountService.DeleteDiscount(discount);
            return RedirectToAction("Index");
        }
    }
}