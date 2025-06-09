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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;
        private readonly IReportService _reportService;
        private readonly IReviewService _reviewService;
        private readonly IBookService _bookService;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, ICategoryService categoryService, ICartService cartService, IReportService reportService, IReviewService reviewService, IBookService bookService)
        {
            _logger = logger;
            _UnitOfWork = unitOfWork;
            _categoryService = categoryService;
            _cartService = cartService;
            _reportService = reportService;
            _reviewService = reviewService;
            _bookService = bookService;
        }

        public async Task<IActionResult> Index(int? page)
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
                //Thêm vào mới nha bà già Nhi
                //Lấy 1 quyển sách đầu tiên theo category
                Dictionary<Guid, Book> firstBooksByCategory = new Dictionary<Guid, Book>();
                var categories = await _UnitOfWork.Category.GetAllAsync();
                //book và category quan hệ nhiều - nhiều
                foreach (var category in categories)
                {
                    //Lấy sách đầu tiên trong mỗi category
                    var firstBook = await _UnitOfWork.Book.GetAsync(c => c.Categories.Any(cat => cat.CategoryId == category.CategoryId), "Authors");
                    if (firstBook != null)
                    {
                        firstBooksByCategory[category.CategoryId] = firstBook;
                    }
                }
                ViewData["FirstBooksByCategory"] = firstBooksByCategory;
                //Hết rồi nha bé Nhi
                //Lấy sách theo số lượng sách đã bán nhiều nhất trong order
                var bestSellingBooksReport = await _reportService.GetBestSellingBooksAsync(null, null, 6);
                //in ra để debug
                foreach (var book in bestSellingBooksReport)
                {
                    Console.WriteLine($"BookId: {book.BookId}, Title: {book.Title}, TotalSold: {book.QuantitySold}");
                }
                //chuyển thành List<Book> để truyền vào ViewComponent
                List<BookRatingViewModel> bestSellingBooks = new List<BookRatingViewModel>();
                foreach (var b in bestSellingBooksReport)
                {
                    var bookData = await _UnitOfWork.Book.GetAsync(c => c.BookId == b.BookId, "Authors");
                    if (bookData != null)
                    {
                        bestSellingBooks.Add(new BookRatingViewModel
                        {
                            Book = new Book
                            {
                                BookId = b.BookId,
                                BookTitle = b.Title,
                                ImageURL = bookData.ImageURL,
                                Authors = bookData.Authors,
                                Price = bookData.Price,
                                Stock = bookData.Stock
                            },
                            AverageRating = await _reviewService.GetAverageRatingForBookAsync(b.BookId)
                        });
                    }
                }
                var recommendBooksReport = await _reportService.GetHighestRatedBooksAsync(null, null, 6);
                //chuyển thành List<Book> để truyền vào ViewComponent
                List<BookRatingViewModel> recommendBooks = new List<BookRatingViewModel>();
                foreach (var b in recommendBooksReport)
                {
                    var bookData = await _UnitOfWork.Book.GetAsync(c => c.BookId == b.BookId, "Authors");
                    if (bookData != null)
                    {
                        recommendBooks.Add(new BookRatingViewModel
                        {
                            Book = new Book
                            {
                                BookId = b.BookId,
                                BookTitle = b.Title,
                                ImageURL = bookData.ImageURL,
                                Authors = bookData.Authors,
                                Price = bookData.Price,
                                Stock = bookData.Stock
                            },
                            AverageRating = await _reviewService.GetAverageRatingForBookAsync(b.BookId)
                        });
                    }
                }
                var newReleaseBooks = await _bookService.GetBookNewReleasesAsync(3);
                var homeVM = new HomeVM
                {
                    Categories = await _UnitOfWork.Category.GetAllAsync(),
                    BestSellerBooks = bestSellingBooks,
                    RecommendBooks = recommendBooks,
                    NewReleaseBooks = newReleaseBooks,
                    Page = page ?? 1
                };
                //
                return View(homeVM);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                ViewBag.CartCount = 0;
                return View("Error");
            }
        }
    }
}