using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StackBook.DAL.IRepository;
using StackBook.Data;
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

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, CategoryService categoryService)
        {
            _logger = logger;
            _UnitOfWork = unitOfWork;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
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

            return View(homeVM);
        }
    }
}
