using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StackBook.Data;
using StackBook.Models;
using StackBook.Services;

namespace StackBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookService _bookService;

        public HomeController(ILogger<HomeController> logger, BookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        public IActionResult Index()
        {
            List<Book> books = _bookService.GetAllBooks();
            return View(books);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
