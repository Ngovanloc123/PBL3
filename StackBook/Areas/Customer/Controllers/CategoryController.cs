using Microsoft.AspNetCore.Mvc;
using StackBook.Services;
using StackBook.ViewModels;

namespace StackBook.Areas.Customer.Controllers
{
    // Định nghĩa khu vực cho controller
    [Area("Customer")]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoryController(BookService bookService, CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var viewModel = _categoryService.GetCategoriesWithBooks();

            return View(viewModel);
        }

        public IActionResult Books()
        {
            return View();
        }

        public IActionResult ProductDetail()
        {
            return View();
        }
    }
}
