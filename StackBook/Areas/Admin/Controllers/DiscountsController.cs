using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using StackBook.Interfaces;
using StackBook.Models;
using System;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace StackBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class DiscountsController : Controller
    {
        private readonly IDiscountService _discountService;

        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? page)
        {
            try
            {
                int pageSize = 8;
                int pageNumber = page ?? 1;

                var discounts = await _discountService.GetAllDiscounts();

                var pagedDiscounts = discounts.ToPagedList(pageNumber, pageSize);
                return View(pagedDiscounts);
            }
            catch
            {
                TempData["error"] = "An error occurred while retrieving discounts.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var discount = (await _discountService.GetAllDiscounts())
                    .Find(d => d.DiscountId == id);
                
                return discount == null 
                    ? NotFoundWithRedirect("Discount not found.") 
                    : View(discount);
            }
            catch
            {
                return ErrorWithRedirect("An error occurred while retrieving discount details.");
            }
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Discount discount)
        {
            if (!ModelState.IsValid)
                return View(discount);

            try
            {
                discount.DiscountId = Guid.NewGuid();
                discount.CreatedDiscount = DateTime.Now;
                
                await _discountService.CreateDiscount(discount);
                
                TempData["success"] = "Discount created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while creating the discount.");
                return View(discount);
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var discount = (await _discountService.GetAllDiscounts())
                    .Find(d => d.DiscountId == id);
                
                return discount == null 
                    ? NotFoundWithRedirect("Discount not found.") 
                    : View(discount);
            }
            catch
            {
                return ErrorWithRedirect("An error occurred while retrieving the discount.");
            }
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] Discount discount)
        {
            if (id != discount.DiscountId)
                return NotFoundWithRedirect("Discount ID mismatch.");

            if (!ModelState.IsValid)
                return View(discount);

            try
            {
                await _discountService.UpdateDiscount(discount);
                TempData["success"] = "Discount updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while updating the discount.");
                return View(discount);
            }
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var discount = (await _discountService.GetAllDiscounts())
                    .Find(d => d.DiscountId == id);
                
                return discount == null 
                    ? NotFoundWithRedirect("Discount not found.") 
                    : View(discount);
            }
            catch
            {
                return ErrorWithRedirect("An error occurred while retrieving the discount.");
            }
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var discount = (await _discountService.GetAllDiscounts())
                    .Find(d => d.DiscountId == id);

                if (discount != null)
                {
                    await _discountService.DeleteDiscount(discount);
                    TempData["success"] = "Discount deleted successfully!";
                }
                else
                {
                    TempData["error"] = "Discount not found.";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return ErrorWithRedirect("An error occurred while deleting the discount.");
            }
        }

        private IActionResult NotFoundWithRedirect(string message)
        {
            TempData["error"] = message;
            return RedirectToAction(nameof(Index));
        }

        private IActionResult ErrorWithRedirect(string message)
        {
            TempData["error"] = message;
            return RedirectToAction(nameof(Index));
        }
    }
}