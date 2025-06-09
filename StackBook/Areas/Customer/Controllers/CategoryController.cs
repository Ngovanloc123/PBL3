// using DocumentFormat.OpenXml.Drawing.Diagrams;
// using Microsoft.AspNetCore.Mvc;
// using StackBook.DAL;
// using StackBook.DAL.IRepository;
// using StackBook.Services;
// using StackBook.Models;
// using StackBook.ViewModels;
// using DocumentFormat.OpenXml.VariantTypes;
// using Microsoft.AspNetCore.Authorization;
// using StackBook.Interfaces;

// namespace StackBook.Areas.Customer.Controllers
// {
//     //[Authorize(Roles = "Customer")]
//     [Area("Customer")]
//     public class CategoryController : Controller
//     {
//         private readonly IUnitOfWork _unitOfWork;
//         private readonly ISearchService _searchService;
//         private readonly ICategoryService _categoryService;
//         private readonly IReviewRepository _reviewRepository;


//         public CategoryController(IUnitOfWork unitOfWork, ISearchService searchService, ICategoryService categoryService, IReviewRepository reviewRepository)
//         {
//             _unitOfWork = unitOfWork;
//             _searchService = searchService;
//             _categoryService = categoryService;
//             _reviewRepository = reviewRepository;
//         }

//         public async Task<IActionResult> Index(Guid? categoryId)
//         {
//             if (categoryId != null)
//             {
//                 var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == categoryId, "Books.Authors");
//                 ViewBag.Category = category;
//                 return View(category);
//             }
//             else
//             {
//                 var categories = await _unitOfWork.Category.GetAllAsync("Books.Authors");
//                 return View(categories);
//             }
//         }



//         public async Task<IActionResult> BookDetail(Guid? bookId)
//         {
//             var book = await _unitOfWork.Book.GetAsync(b => b.BookId == bookId, "Authors,Categories.Books.Authors");
//             ViewBag.Book = book;
//             //Kiểm tra book có bao nhiêu rating
//             if (book == null)
//             {
//                 TempData["error"] = "Book not found";
//                 return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: Book not found", StatusCode = 404 });
//             }
//             if (book.Authors == null || book.Authors.Count == 0)
//             {
//                 TempData["error"] = "Book has no authors";
//                 return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: Book has no authors", StatusCode = 404 });
//             }
//             var countBookRating = await _reviewRepository.GetReviewCountForBookAsync(bookId.Value);
//             var categoriesBook = await _unitOfWork.Category.GetAsync(c => c.Books.Any(b => b.BookId == bookId), "Books.Authors");
//             if (categoriesBook == null)
//             {
//                 TempData["error"] = "Book has no categories";
//                 return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: Book has no categories", StatusCode = 404 });
//             }
//             var averareRatingBook = await _reviewRepository.GetAverageRatingForBookAsync(bookId.Value);
//             var soldBook = await _unitOfWork.Book.CountBooksSoldAsync(bookId.Value);
//             //hiển thị hết tất cả review của sách này
//             var reviews = await _reviewRepository.GetByBookIdAsync(bookId.Value);
//             Dictionary<Guid, string> userNames = new Dictionary<Guid, string>();
//             foreach (var review in reviews)
//             {
//                 if (!userNames.ContainsKey(review.UserId))
//                 {
//                     var user = await _unitOfWork.User.GetAsync(u => u.UserId == review.UserId);
//                     if (user != null)
//                     {
//                         userNames[review.UserId] = user.FullName ?? "Unknown User"; // Default to "Unknown User" if UserName is null
//                     }
//                     else
//                     {
//                         userNames[review.UserId] = "Unknown User"; // Default to "Unknown User" if user not found
//                     }
//                 }
//             }
//             Dictionary<Guid, string> avtForUsers = new Dictionary<Guid, string>();
//             foreach (var user in userNames)
//             {
//                 var userAvt = await _unitOfWork.User.GetAsync(u => u.UserId == user.Key);
//                 if (userAvt != null && !string.IsNullOrEmpty(userAvt.AvatarUrl))
//                 {
//                     avtForUsers[user.Key] = userAvt.AvatarUrl;
//                 }
//                 else
//                 {
//                     avtForUsers[user.Key] = "/images/default-avatar.png"; // Default avatar if not found
//                 }
//             }
//             Dictionary<Guid, double> bookRatingForId = new Dictionary<Guid, double>();
//             //tính rating trung bình của mỗi quyển sách trong cùng thể loại
//             foreach (var bookInCategory in categoriesBook.Books)
//             {
//                 var averageRating = await _reviewRepository.GetAverageRatingForBookAsync(bookInCategory.BookId);
//                 if (averageRating.HasValue)
//                 {
//                     bookRatingForId[bookInCategory.BookId] = averageRating.Value;
//                 }
//                 else
//                 {
//                     bookRatingForId[bookInCategory.BookId] = 0; // Default to 0 if no ratings
//                 }
//             }
//             // Sort the books by rating in descending order
//             var sortedBooksByRating = bookRatingForId.OrderByDescending(b => b.Value).ToList();
//             //in ra các quyển sách trong cùng thể loại theo rating
//             Console.WriteLine("Books in the same category sorted by rating:");
//             foreach (var bookRating in sortedBooksByRating)
//             {
//                 var bookInCategory = categoriesBook.Books.FirstOrDefault(b => b.BookId == bookRating.Key);
//                 if (bookInCategory != null)
//                 {
//                     Console.WriteLine($"Book: {bookInCategory.BookTitle}, Rating: {bookRating.Value}");
//                 }
//             }
//             Console.WriteLine($"Count book rating: {countBookRating}");
//             ViewData["CountRatingBook"] = countBookRating;
//             ViewData["CategoriesBook"] = categoriesBook.CategoryName;
//             ViewData["SoldBook"] = soldBook;
//             ViewData["AverageRatingBook"] = averareRatingBook;
//             ViewData["Reviews"] = reviews;
//             ViewData["UserNames"] = userNames;
//             ViewData["AvtForUsers"] = avtForUsers;
//             ViewData["BookRatingForId"] = bookRatingForId;
//             // Get books in the same category (excluding current book)
//             var sameCategoryBooks = new List<Book>();
//             if (book.Categories != null && book.Categories.Any())
//             {
//                 var category = book.Categories.First();
//                 sameCategoryBooks = (await _unitOfWork.Category.GetAsync(
//                     c => c.CategoryId == category.CategoryId,
//                     "Books.Authors"))?.Books
//                     .Where(b => b.BookId != bookId)
//                     .Take(4)
//                     .ToList();
//             }
//             ViewData["SameCategoryBooks"] = sameCategoryBooks;
//             return View(book);
//         }


//         public async Task<IActionResult> Search(string? s)
//         {


//             //if (string.IsNullOrWhiteSpace(s))
//             //{
//             //    ViewBag.Query = string.Empty;
//             //    return View(new List<Book>());
//             //}

//             ViewBag.Query = s;
//             var booksSearch = await _searchService.SearchBooksAsync(s);
//             return View(booksSearch);
//         }


//     }
// }
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
using StackBook.DAL.Repository;
using Newtonsoft.Json;

namespace StackBook.Areas.Customer.Controllers
{
    //[Authorize(Roles = "Customer")]
    [Area("Customer")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISearchService _searchService;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;
        private readonly IReviewRepository _reviewRepository;


        public CategoryController(IUnitOfWork unitOfWork, ISearchService searchService, ICategoryService categoryService, IReviewRepository reviewRepository, IReviewService reviewService)
        {
            _unitOfWork = unitOfWork;
            _searchService = searchService;
            _categoryService = categoryService;
            _reviewRepository = reviewRepository;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index(Guid? categoryId)
        {
            if (categoryId != null)
            {
                var bookRatings = new List<BookRatingViewModel>();

                // Lấy thông tin category theo id (bao gồm sách và tác giả)
                var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == categoryId, "Books.Authors");
                ViewBag.CategoryName = category.CategoryName;

                foreach (var book in category.Books)
                {
                    // Lấy điểm đánh giá trung bình cho từng sách
                    var averageRating = await _reviewService.GetAverageRatingForBookAsync(book.BookId);
                    bookRatings.Add(new BookRatingViewModel
                    {
                        Book = book,
                        AverageRating = averageRating
                    });
                }

                return View(bookRatings); // Trả về view riêng nếu bạn có
            }
            else
            {
                var categories = await _unitOfWork.Category.GetAllAsync("Books.Authors");
                var categoryBookList = new List<CategoryVM>();

                foreach (var category in categories)
                {
                    var bookRatings = new List<BookRatingViewModel>();

                    foreach (var book in category.Books)
                    {
                        var averageRating = await _reviewService.GetAverageRatingForBookAsync(book.BookId);
                        bookRatings.Add(new BookRatingViewModel
                        {
                            Book = book,
                            AverageRating = averageRating
                        });
                    }

                    categoryBookList.Add(new CategoryVM
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        BookRatings = bookRatings
                    });
                }

                return View(categoryBookList); // Trả về view tổng hợp tất cả category
            }
        }




        public async Task<IActionResult> BookDetail(Guid? bookId)
        {
            var book = await _unitOfWork.Book.GetAsync(b => b.BookId == bookId, "Authors,Categories.Books.Authors");
            ViewBag.Book = book;
            //Kiểm tra book có bao nhiêu rating
            if (book == null)
            {
                TempData["error"] = "Book not found";
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: Book not found", StatusCode = 404 });
            }
            if (book.Authors == null || book.Authors.Count == 0)
            {
                TempData["error"] = "Book has no authors";
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: Book has no authors", StatusCode = 404 });
            }
            var countBookRating = await _reviewRepository.GetReviewCountForBookAsync(bookId.Value);
            var categoriesBook = await _unitOfWork.Category.GetAsync(c => c.Books.Any(b => b.BookId == bookId), "Books.Authors");
            if (categoriesBook == null)
            {
                TempData["error"] = "Book has no categories";
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: Book has no categories", StatusCode = 404 });
            }
            var averareRatingBook = await _reviewRepository.GetAverageRatingForBookAsync(bookId.Value);
            var soldBook = await _unitOfWork.Book.CountBooksSoldAsync(bookId.Value);
            //hiển thị hết tất cả review của sách này
            var reviews = await _reviewRepository.GetByBookIdAsync(bookId.Value);
            Dictionary<Guid, string> userNames = new Dictionary<Guid, string>();
            foreach (var review in reviews)
            {
                if (!userNames.ContainsKey(review.UserId))
                {
                    var user = await _unitOfWork.User.GetAsync(u => u.UserId == review.UserId);
                    if (user != null)
                    {
                        userNames[review.UserId] = user.FullName ?? "Unknown User"; // Default to "Unknown User" if UserName is null
                    }
                    else
                    {
                        userNames[review.UserId] = "Unknown User"; // Default to "Unknown User" if user not found
                    }
                }
            }
            Dictionary<Guid, string> avtForUsers = new Dictionary<Guid, string>();
            foreach (var user in userNames)
            {
                var userAvt = await _unitOfWork.User.GetAsync(u => u.UserId == user.Key);
                if (userAvt != null && !string.IsNullOrEmpty(userAvt.AvatarUrl))
                {
                    avtForUsers[user.Key] = userAvt.AvatarUrl;
                }
                else
                {
                    avtForUsers[user.Key] = "/images/default-avatar.png"; // Default avatar if not found
                }
            }
            Dictionary<Guid, double> bookRatingForId = new Dictionary<Guid, double>();
            //tính rating trung bình của mỗi quyển sách trong cùng thể loại
            foreach (var bookInCategory in categoriesBook.Books)
            {
                var averageRating = await _reviewRepository.GetAverageRatingForBookAsync(bookInCategory.BookId);
                if (averageRating.HasValue)
                {
                    bookRatingForId[bookInCategory.BookId] = averageRating.Value;
                }
                else
                {
                    bookRatingForId[bookInCategory.BookId] = 0; // Default to 0 if no ratings
                }
            }
            // Sort the books by rating in descending order
            var sortedBooksByRating = bookRatingForId.OrderByDescending(b => b.Value).ToList();
            //in ra các quyển sách trong cùng thể loại theo rating
            Console.WriteLine("Books in the same category sorted by rating:");
            foreach (var bookRating in sortedBooksByRating)
            {
                var bookInCategory = categoriesBook.Books.FirstOrDefault(b => b.BookId == bookRating.Key);
                if (bookInCategory != null)
                {
                    Console.WriteLine($"Book: {bookInCategory.BookTitle}, Rating: {bookRating.Value}");
                }
            }
            Console.WriteLine($"Count book rating: {countBookRating}");
            ViewData["CountRatingBook"] = countBookRating;
            ViewData["CategoriesBook"] = categoriesBook.CategoryName;
            ViewData["SoldBook"] = soldBook;
            ViewData["AverageRatingBook"] = averareRatingBook;
            ViewData["Reviews"] = reviews;
            ViewData["UserNames"] = userNames;
            ViewData["AvtForUsers"] = avtForUsers;
            ViewData["BookRatingForId"] = bookRatingForId;
            // Get books in the same category (excluding current book)
            var sameCategoryBooks = new List<Book>();
            if (book.Categories != null && book.Categories.Any())
            {
                var category = book.Categories.First();
                sameCategoryBooks = (await _unitOfWork.Category.GetAsync(
                    c => c.CategoryId == category.CategoryId,
                    "Books.Authors"))?.Books
                    .Where(b => b.BookId != bookId)
                    .Take(4)
                    .ToList();
            }
            ViewData["SameCategoryBooks"] = sameCategoryBooks;
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


        public async Task<IActionResult> BuyNow(Guid bookId, int quantity)
        {
            try
            {
                var userId = Request.Cookies["userId"];
                var user = await _unitOfWork.User.GetAsync(u => u.UserId == Guid.Parse(userId), "ShippingAddresses");

                var selectedBooks = new List<SelectedBook>();
                var book = await _unitOfWork.Book.GetAsync(b => b.BookId == bookId);
                selectedBooks.Add(new SelectedBook
                {
                    Book = book,
                    Quantity = quantity
                });

                var shippingAddressDefault = await _unitOfWork.ShippingAddress.GetAsync(sa => sa.UserId == Guid.Parse(userId));
                var checkoutRequestNew = new CheckoutRequest
                {
                    User = user,
                    SelectedBooks = selectedBooks,
                    shippingAddressDefault = shippingAddressDefault,

                    discounts = (await _unitOfWork.Discount.GetListAsync(d => d.DiscountCode != "0")).ToList()
                };

                var jsonSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                HttpContext.Session.SetString("CheckoutRequest", JsonConvert.SerializeObject(checkoutRequestNew, jsonSettings));

                return RedirectToAction("Checkout", "Cart");
            }
            catch (Exception ex)
            {
                return View("Error", new { message = "Buy now error", error = ex.Message });
            }
        }

        }
}