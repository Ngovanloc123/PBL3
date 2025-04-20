using Microsoft.AspNetCore.Mvc;
using StackBook.DAL;
using StackBook.DAL.IRepository;
using StackBook.Services;
using StackBook.ViewModels;

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
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = _categoryService.GetNameById(categoryId);

            if (categoryId != null)
            {
                var books = _unitOfWork.Category.GetBooksByCategoryId(categoryId);
                return View(books);
            }
            else
            {
                var CategoriesWithBook = _unitOfWork.Category.GetCategoriesWithBooks();
                return View(CategoriesWithBook);
            }
        }

        public IActionResult BookDetail(Guid? bookId)
        {
            var allBooks = _unitOfWork.Book.GetAllBookWithAuthor();
            var bookDetail = _unitOfWork.Book.GetBookWithAuthor(bookId);

            var viewModel = new BookDetailPageViewModel
            {
                AllBooks = allBooks,
                BookDetail = bookDetail
            };

            return View(viewModel);
        }


        public IActionResult Search(String? s)
        {
            ViewBag.Query = s;
            var allBooks = _unitOfWork.Book.GetAllBookWithAuthor();
            var booksSearch = _searchService.SearchBooks(allBooks, s);
            return View(booksSearch);
        }


    }
}
