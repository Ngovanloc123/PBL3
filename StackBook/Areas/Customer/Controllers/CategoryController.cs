using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Mvc;
using StackBook.DAL;
using StackBook.DAL.IRepository;
using StackBook.Services;
using StackBook.Models;
using StackBook.ViewModels;
using DocumentFormat.OpenXml.VariantTypes;

namespace StackBook.Areas.Customer.Controllers
{
    // Định nghĩa khu vực cho controller
    [Area("Customer")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SearchService _searchService;
        private readonly CategoryService _categoryService;
        

        public CategoryController(IUnitOfWork unitOfWork, SearchService searchService, CategoryService categoryService)
        {
            _unitOfWork = unitOfWork;
            _searchService = searchService;
            _categoryService = categoryService;
        }

        public IActionResult Index(Guid? categoryId)
        {

            if (categoryId != null)
            {
                var category = _unitOfWork.Category.Get(c => c.CategoryId == categoryId);
                ViewBag.Category = category;
                return View(category);
            }
            else
            {
                var categories = _unitOfWork.Category.GetAll().ToList();
                return View(categories);
            }
        }

        public IActionResult BookDetail(Guid? bookId)
        {
            var book = _unitOfWork.Book.Get(b => b.BookId == bookId);

            return View(book);
        }


        public IActionResult Search(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                ViewBag.Query = string.Empty;
                return View(new List<Book>());
            }

            ViewBag.Query = s;
            var booksSearch = _searchService.SearchBooks(s);
            return View(booksSearch);
        }


    }
}
