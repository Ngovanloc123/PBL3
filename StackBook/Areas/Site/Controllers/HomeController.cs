using System.Diagnostics;
using System.Security.Claims;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Services;
using StackBook.ViewModels;

namespace StackBook.Areas.Site.Controllers
{
    [Area("Site")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly CategoryService _categoryService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, CategoryService categoryService, ICartService cartService)
        {
            _logger = logger;
            _UnitOfWork = unitOfWork;
            _categoryService = categoryService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Statistic", new { area = "Admin" });
                }
            }

            var homeVM = new HomeVM
            {

                Categories = await _UnitOfWork.Category.GetAllAsync(),
                Books = await _UnitOfWork.Book.GetAllAsync("Authors")
            };


            try
            {
                // Lấy userId từ claims
                var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserIdClaims))
                {
                    ViewBag.CartCount = 0;
                }
                else
                {
                    var currentUserId = Guid.Parse(currentUserIdClaims);
                    ViewBag.CartCount = await _cartService.GetCartCount(currentUserId);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                ViewBag.CartCount = 0;
            }

            return View(homeVM);
        }
            


        
    }
}
