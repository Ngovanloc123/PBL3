using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Mvc;
using StackBook.DAL;
using StackBook.DAL.IRepository;
using StackBook.Services;
using StackBook.Models;
using StackBook.ViewModels;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Authorization;
using StackBook.Interfaces;

namespace StackBook.Areas.Customer.Controllers
{
    //[Authorize(Roles = "Customer")]
    [Area("Customer")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISearchService _searchService;
        private readonly ICategoryService _categoryService;
        

        public CategoryController(IUnitOfWork unitOfWork, ISearchService searchService, ICategoryService categoryService)
        {
            _unitOfWork = unitOfWork;
            _searchService = searchService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(Guid? categoryId)
        {
            if (categoryId != null)
            {
                var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == categoryId, "Books.Authors");
                ViewBag.Category = category;
                return View(category);
            }
            else
            {
                var categories = await _unitOfWork.Category.GetAllAsync("Books.Authors");
                return View(categories);
            }
        }

        public async Task<IActionResult> BookDetail(Guid? bookId)
        {
            var book = await _unitOfWork.Book.GetAsync(b => b.BookId == bookId, "Authors,Categories.Books.Authors");

            return View(book);
        }


        public async Task<IActionResult> Search(string? s)
        {
            //if (string.IsNullOrWhiteSpace(s))
            //{
            //    ViewBag.Query = string.Empty;
            //    return View(new List<Book>());
            //}

            ViewBag.Query = s;
            var booksSearch = await _searchService.SearchBooksAsync(s);
            return View(booksSearch);
        }


    }
}
