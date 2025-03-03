using Microsoft.AspNetCore.Mvc;
using StackBook.Services;

namespace StackBook.Controllers
{
    public class CategoryController : Controller
    {

        private readonly BookService _bookService;
        public CategoryController(BookService bookService)
        {
            _bookService = bookService;
        }

        public IActionResult Index()
        {
            //var listCategories = new 
            return View();
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
