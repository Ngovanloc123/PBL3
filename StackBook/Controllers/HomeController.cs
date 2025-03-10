using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StackBook.Data;
using StackBook.Models;
using StackBook.Services;
using StackBook.ViewModels;

namespace StackBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookService _bookService;
        private readonly CategoryService _categoryService;

        public HomeController(ILogger<HomeController> logger, BookService bookService, CategoryService categoryService)
        {
            _logger = logger;
            _bookService = bookService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var dataHome = new DataHomeViewModel
            {
                Books = _bookService.GetAllBooks(),
                MenuCategories = _categoryService.GetMenuCategories()
            };

            return View(dataHome);
        }
    }
}
