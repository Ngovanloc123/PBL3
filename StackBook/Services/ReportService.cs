using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using StackBook.Exceptions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using StackBook.DAL.IRepository;
using StackBook.DAL.Repository;
using StackBook.ViewModels;
using System.Globalization;

namespace StackBook.Services
{
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBookService _bookService;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewService _reviewService;
        private readonly IUnitOfWork _unitOfWork;
        public ReportService(
            IOrderRepository orderRepository,
            IOrderService orderService,
            ICategoryRepository categoryRepository,
            IAuthorRepository authorRepository,
            IBookRepository bookRepository,
            IBookService bookService,
            IUserRepository userRepository,
            IUserService userService,
            IReviewRepository reviewRepository,
            IReviewService reviewService,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _orderService = orderService;
            _categoryRepository = categoryRepository;
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _bookService = bookService;
            _userRepository = userRepository;
            _userService = userService;
            _reviewRepository = reviewRepository;
            _reviewService = reviewService;
            _unitOfWork = unitOfWork;
        }
        //th?ng kê t?ng s? ??n hàng
        public async Task<int> GetTotalOrdersAsync(DateTime? start, DateTime? end)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (start.HasValue && end.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value).ToList();
            }
            return orders.Count;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (orders == null || !orders.Any())
            {
                throw new Exception("No orders found.");
            }
            return orders;
        }
        //th?ng kê ??n hàng theo status
        //status: 1. Pending, 2. Shipped, 3. Canceled, 4. Delivered, 5. Return
        public async Task<List<Order>> GetOrdersByStatusAsync(int status, DateTime? start, DateTime? end)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status);
            if (start.HasValue && end.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value).ToList();
            }
            if (orders == null || !orders.Any())
            {
                throw new Exception($"No orders found with status {status}.");
            }
            return orders;
        }
        //th?ng kê ??n hàng ?ã hoàn thành
        public async Task<List<Order>> GetCompletedOrdersAsync(DateTime? start, DateTime? end)
        {
            return await GetOrdersByStatusAsync(4, start, end); // 4. Delivered
        }
        //th?ng kê ??n hàng ?ang x? lý
        public async Task<List<Order>> GetProcessingOrdersAsync(DateTime? start, DateTime? end)
        {
            return await GetOrdersByStatusAsync(1, start, end); // 1. Pending
        }
        //th?ng kê ??n hàng ?ã h?y
        public async Task<List<Order>> GetCanceledOrdersAsync(DateTime? start, DateTime? end)
        {
            return await GetOrdersByStatusAsync(3, start, end); // 3. Canceled
        }
        //th?ng kê ??n hàng ?ã tr?
        public async Task<List<Order>> GetReturnedOrdersAsync(DateTime? start, DateTime? end)
        {
            return await GetOrdersByStatusAsync(5, start, end); // 5. Return
        }
        //th?ng kê ??n hàng ?ang giao
        public async Task<List<Order>> GetShippingOrdersAsync(DateTime? start, DateTime? end)
        {
            return await GetOrdersByStatusAsync(2, start, end); // 2. Shipped
        }
        //th?ng kê doanh thu t?ng
        public async Task<double> GetTotalRevenueAsync(DateTime? start, DateTime? end)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (start.HasValue && end.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value).ToList();
            }
            if (orders == null || !orders.Any())
            {
                throw new Exception("No orders found.");
            }
            return orders.Sum(o => o.TotalPrice);
        }
        //th?ng kê doanh thu theo ngày/tu?n/tháng/kho?ng th?i gian
        public async Task<List<RevenueByDateViewModel>> GetRevenueByDateAsync(DateTime? start, DateTime? end, TimeRangeType rangeType)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (start.HasValue && end.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value).ToList();
            }
            if (orders == null || !orders.Any())
            {
                throw new Exception("No orders found.");
            }

            var revenueByDate = new List<RevenueByDateViewModel>();
            switch (rangeType)
            {
                case TimeRangeType.Daily:
                    revenueByDate = orders.GroupBy(o => o.CreatedAt.Date)
                        .Select(g => new RevenueByDateViewModel
                        {
                            Date = g.Key,
                            Revenue = g.Sum(o => o.TotalPrice)
                        }).ToList();
                    break;
                case TimeRangeType.Weekly:
                    revenueByDate = orders.GroupBy(o => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(o.CreatedAt, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                        .Select(g => new RevenueByDateViewModel
                        {
                            Date = g.First().CreatedAt.Date,
                            Revenue = g.Sum(o => o.TotalPrice)
                        }).ToList();
                    break;
                case TimeRangeType.Monthly:
                    revenueByDate = orders.GroupBy(o => new DateTime(o.CreatedAt.Year, o.CreatedAt.Month, 1))
                        .Select(g => new RevenueByDateViewModel
                        {
                            Date = g.Key,
                            Revenue = g.Sum(o => o.TotalPrice)
                        }).ToList();
                    break;
                case TimeRangeType.Custom:
                    // Handle custom range logic if needed
                    break;
            }
            return revenueByDate;
        }
        //th?ng kê t? l? h?y ??n hàng
        public async Task<double> GetCancelRateAsync(DateTime? start, DateTime? end)
        {
            var canceledOrders = await GetCanceledOrdersAsync(start, end);
            var totalOrders = await GetTotalOrdersAsync(start, end);
            if (totalOrders == 0)
            {
                return 0; // Tránh chia cho 0
            }
            return (double)canceledOrders.Count / totalOrders * 100; // T? l? ph?n tr?m
        }
        //th?ng kê top khách hàng mua nhi?u nh?t
        public async Task<List<TopCustomerViewModel>> GetTopCustomersAsync(DateTime? start, DateTime? end, int topCount)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (start.HasValue && end.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value).ToList();
            }
            if (orders == null || !orders.Any())
            {
                throw new Exception("No orders found.");
            }
            var topCustomers = orders.GroupBy(o => o.UserId)
                .Select(g => new TopCustomerViewModel
                {
                    UserId = g.Key,
                    TotalSpent = g.Sum(o => o.TotalPrice)
                }).OrderByDescending(o => o.TotalSpent).Take(topCount).ToList();
            return topCustomers;
        }
        //th?ng kê doanh thu theo th? lo?i sách
        public async Task<List<RevenueByCategoryViewModel>> GetRevenueByCategoryAsync(DateTime? start, DateTime? end)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (start.HasValue && end.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value).ToList();
            }
            if (orders == null || !orders.Any())
            {
                throw new Exception("No orders found.");
            }

            var categoryRevenues = new List<RevenueByCategoryViewModel>();
            var books = await _bookRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            foreach (var category in categories)
            {
                var revenue = orders.Where(o => o.OrderDetails != null && o.OrderDetails.Any(od => books.Any(b => b.BookId == od.BookId && b.Categories.Any(c => c.CategoryId == category.CategoryId)))).Sum(o => o.TotalPrice);
                categoryRevenues.Add(new RevenueByCategoryViewModel
                {
                    CategoryName = category.CategoryName,
                    Revenue = revenue
                });
            }
            return categoryRevenues;
        }
        //th?ng kê sách s? l??ng sách theo th? lo?i:
        public async Task<List<BookCountByCategoryViewModel>> GetBookCountByCategoryAsync(DateTime? start, DateTime? end)
        {
            var books = await _bookRepository.GetAllAsync();
            if (books == null || !books.Any())
            {
                throw new Exception("No books found.");
            }

            var bookCountByCategory = new List<BookCountByCategoryViewModel>();
            var categories = await _categoryRepository.GetAllAsync();

            foreach (var category in categories)
            {
                var bookCount = books.Count(b => b.Categories.Any(c => c.CategoryId == category.CategoryId));
                bookCountByCategory.Add(new BookCountByCategoryViewModel
                {
                    CategoryName = category.CategoryName,
                    BookCount = bookCount
                });
            }
            return bookCountByCategory;
        }
        //th?ng kê sách bán ch?y nh?t
        public async Task<List<BookSaleInfoViewModel>> GetBestSellingBooksAsync(DateTime? start, DateTime? end, int topCount)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (start.HasValue && end.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value).ToList();
            }
            if (orders == null || !orders.Any())
            {
                throw new Exception("No orders found.");
            }

            var bookSales = new List<BookSaleInfoViewModel>();
            var books = await _bookRepository.GetAllAsync();

            foreach (var book in books)
            {
                var quantitySold = orders.Sum(o => o.OrderDetails?.Where(od => od.BookId == book.BookId).Sum(od => od.Quantity) ?? 0);
                if (quantitySold > 0)
                {
                    bookSales.Add(new BookSaleInfoViewModel
                    {
                        BookId = book.BookId,
                        Title = book.BookTitle,
                        QuantitySold = quantitySold
                    });
                }
            }
            return bookSales.OrderByDescending(bs => bs.QuantitySold).Take(topCount).ToList();
        }
        //th?ng kê sách ít bán nh?t
        public async Task<List<BookSaleInfoViewModel>> GetLeastSellingBooksAsync(DateTime? start, DateTime? end, int topCount)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (start.HasValue && end.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value).ToList();
            }
            if (orders == null || !orders.Any())
            {
                throw new Exception("No orders found.");
            }

            var bookSales = new List<BookSaleInfoViewModel>();
            var books = await _bookRepository.GetAllAsync();

            foreach (var book in books)
            {
                var quantitySold = orders.Sum(o => o.OrderDetails?.Where(od => od.BookId == book.BookId).Sum(od => od.Quantity) ?? 0);
                if (quantitySold > 0)
                {
                    bookSales.Add(new BookSaleInfoViewModel
                    {
                        BookId = book.BookId,
                        Title = book.BookTitle,
                        QuantitySold = quantitySold
                    });
                }
            }
            return bookSales.OrderBy(bs => bs.QuantitySold).Take(topCount).ToList();
        }
        //th?ng kê sách ???c ?ánh giá cao nh?t
        public async Task<List<BookRatingInfoViewModel>> GetHighestRatedBooksAsync(DateTime? start, DateTime? end, int topCount)
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }

            var bookRatings = new List<BookRatingInfoViewModel>();
            var books = await _bookRepository.GetAllAsync();

            foreach (var book in books)
            {
                var bookReviews = reviews.Where(r => r.BookId == book.BookId);
                if (bookReviews.Any())
                {
                    var averageRating = bookReviews.Average(r => r.Rating);
                    var totalReviews = bookReviews.Count();
                    bookRatings.Add(new BookRatingInfoViewModel
                    {
                        BookId = book.BookId,
                        Title = book.BookTitle,
                        AverageRating = averageRating,
                        TotalReviews = totalReviews
                    });
                }
            }
            return bookRatings.OrderByDescending(br => br.AverageRating).Take(topCount).ToList();
        }
        //th?ng kê sách b? ?ánh giá th?p nh?t
        public async Task<List<BookRatingInfoViewModel>> GetLowestRatedBooksAsync(DateTime? start, DateTime? end, int topCount)
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }

            var bookRatings = new List<BookRatingInfoViewModel>();
            var books = await _bookRepository.GetAllAsync();
            foreach (var book in books)
            {
                var bookReviews = reviews.Where(r => r.BookId == book.BookId);
                if (bookReviews.Any())
                {
                    var averageRating = bookReviews.Average(r => r.Rating);
                    var totalReviews = bookReviews.Count();
                    bookRatings.Add(new BookRatingInfoViewModel
                    {
                        BookId = book.BookId,
                        Title = book.BookTitle,
                        AverageRating = averageRating,
                        TotalReviews = totalReviews
                    });
                }
            }
            return bookRatings.OrderBy(br => br.AverageRating).Take(topCount).ToList();
        }
        //th?ng kê t?ng s? ng??i dùng
        public async Task<int> GetTotalUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                throw new Exception("No users found.");
            }
            return users.Count();
        }
        //th?ng kê ng??i dùng m?i trong ngày
        public async Task<int> GetNewUsersTodayAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                throw new Exception("No users found.");
            }
            return users.Count(u => u.CreatedUser.Date == DateTime.UtcNow.Date);
        }
        //th?ng kê ng??i dùng m?i trong tháng
        public async Task<int> GetNewUsersThisMonthAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                throw new Exception("No users found.");
            }
            return users.Count(u => u.CreatedUser.Year == DateTime.UtcNow.Year && u.CreatedUser.Month == DateTime.UtcNow.Month);
        }
        //th?ng kê ng??i dùng có phát sinh ??n hàng
        public async Task<int> GetUsersWithOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (orders == null || !orders.Any())
            {
                throw new Exception("No orders found.");
            }
            return orders.Select(o => o.UserId).Distinct().Count();
        }
        //th?ng kê ng??i dùng có ?ánh giá tích c?c
        public async Task<int> GetPositiveReviewUsersAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }
            return reviews.Where(r => r.Rating >= 4).Select(r => r.UserId).Distinct().Count();
        }
        //th?ng kê ng??i dùng có ?ánh giá tiêu c?c
        public async Task<int> GetNegativeReviewUsersAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }
            return reviews.Where(r => r.Rating <= 2).Select(r => r.UserId).Distinct().Count();
        }
        //thong kê t?ng s? ?ánh giá
        public async Task<int> GetTotalReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }
            return reviews.Count();
        }
        //th?ng kê ?ánh giá trung bình
        public async Task<double> GetAverageRatingAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }
            return reviews.Average(r => r.Rating);
        }
        //th?ng kê t? l? ?ánh giá tích c?c
        public async Task<double> GetPositiveReviewRatePercentAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }
            var positiveReviews = reviews.Count(r => r.Rating >= 4);
            return (double)positiveReviews / reviews.Count * 100; // T? l? ph?n tr?m
        }
        //th?ng kê t? l? ?ánh giá tiêu c?c
        public async Task<double> GetNegativeReviewRatePercentAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }
            var negativeReviews = reviews.Count(r => r.Rating <= 2);
            return (double)negativeReviews / reviews.Count * 100; // T? l? ph?n tr?m
        }
        //th?ng kê sách có nhi?u ?ánh giá nh?t
        public async Task<MostReviewedBookViewModel?> GetMostReviewedBookAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }

            var mostReviewedBook = reviews.GroupBy(r => r.BookId)
                .Select(g => new MostReviewedBookViewModel
                {
                    BookId = g.Key,
                    Title = g.First().Book?.BookTitle ?? "Unknown",
                    TotalReviews = g.Count(),
                    AverageRating = g.Average(r => r.Rating)
                }).OrderByDescending(mr => mr.TotalReviews).FirstOrDefault();

            return mostReviewedBook;
        }
    }
}