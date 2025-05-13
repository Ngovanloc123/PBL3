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

        public IActionResult Index()
        {
            var allBook_category = new AllBookCategoryViewModel
            {
                Categories = _UnitOfWork.Category.GetAllAsync().Result.ToList(),
                Books = _UnitOfWork.Book.GetAllAsync().Result.ToList(),
            };

            return View(allBook_category);
        }
    }
}