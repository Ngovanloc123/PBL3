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
    public class AboutController : Controller
    {
        private readonly ILogger<AboutController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;

        public AboutController(ILogger<AboutController> logger, IUnitOfWork unitOfWork, ICartService cartService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
            }

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

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi load trang About.");
                TempData["error"] = "Có lỗi xảy ra khi tải trang.";
                ViewBag.CartCount = 0;
                return View("Error");
            }
        }
    }
}