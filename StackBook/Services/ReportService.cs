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
        //th?ng k� t?ng s? ??n h�ng
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
        //th?ng k� ??n h�ng theo status
        //status: 1. Pending, 2. Shipped, 3. Canceled, 4. Delivered, 5. Return
        public async Task<List<Order>> GetOrdersByStatusAsync(int status, DateTime? start, DateTime? end)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status);
            if (start.HasValue && end.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value).ToList();
            }
            // if (orders == null || !orders.Any())
            // {
            //     throw new Exception($"No orders found with status {status}.");
            // }
            return orders;
        }
        //th?ng k� ??n h�ng ?� ho�n th�nh
        public async Task<List<Order>> GetCompletedOrdersAsync(DateTime? start, DateTime? end)
        {
            return await GetOrdersByStatusAsync(4, start, end); // 4. Delivered
        }
        //th?ng k� ??n h�ng ?ang x? l�
        public async Task<List<Order>> GetProcessingOrdersAsync(DateTime? start, DateTime? end)
        {
            return await GetOrdersByStatusAsync(1, start, end); // 1. Pending
        }
        //th?ng k� ??n h�ng ?� h?y
        public async Task<List<Order>> GetCanceledOrdersAsync(DateTime? start, DateTime? end)
        {
            return await GetOrdersByStatusAsync(3, start, end); // 3. Canceled
        }
        //th?ng k� ??n h�ng ?� tr?
        public async Task<List<Order>> GetReturnedOrdersAsync(DateTime? start, DateTime? end)
        {
            return await GetOrdersByStatusAsync(5, start, end); // 5. Return
        }
        //th?ng k� ??n h�ng ?ang giao
        public async Task<List<Order>> GetShippingOrdersAsync(DateTime? start, DateTime? end)
        {
            return await GetOrdersByStatusAsync(2, start, end); // 2. Shipped
        }
        //th?ng k� doanh thu t?ng
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
        //th?ng k� doanh thu theo ng�y/tu?n/th�ng/kho?ng th?i gian
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
        //th?ng k� t? l? h?y ??n h�ng
        public async Task<double> GetCancelRateAsync(DateTime? start, DateTime? end)
        {
            var canceledOrders = await GetCanceledOrdersAsync(start, end);
            var totalOrders = await GetTotalOrdersAsync(start, end);
            if (totalOrders == 0)
            {
                return 0; // Tr�nh chia cho 0
            }
            return (double)canceledOrders.Count / totalOrders * 100; // T? l? ph?n tr?m
        }
        //th?ng k� top kh�ch h�ng mua nhi?u nh?t
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
        //th?ng k� doanh thu theo th? lo?i s�ch
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
        //th?ng k� s�ch s? l??ng s�ch theo th? lo?i:
        public async Task<List<BookCountByCategoryViewModel>> GetBookCountByCategoryAsync(DateTime? start, DateTime? end)
        {
            // Lấy tất cả đơn hàng đã hoàn thành (Status = 4) cùng với OrderDetails, Book và Categories
            var orders = await _unitOfWork.Order.GetAllAsync("OrderDetails.Book.Categories");

            // Lọc theo khoảng thời gian (nếu có)
            if (start.HasValue && end.HasValue)
            {
                orders = orders
                    .Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value && o.Status == 4)
                    .ToList();
            }
            else
            {
                orders = orders.Where(o => o.Status == 4).ToList();
            }

            // Lấy tất cả OrderDetails hợp lệ (có Book và Categories)
            var orderDetails = orders
                .Where(o => o.OrderDetails != null)
                .SelectMany(o => o.OrderDetails)
                .Where(od => od.Book != null && od.Book.Categories != null)
                .ToList();

            // Nhóm số lượng sách bán theo từng thể loại
            var categoryCounts = orderDetails
                .SelectMany(od => od.Book.Categories.Select(c => new
                {
                    CategoryName = c.CategoryName,
                    Quantity = od.Quantity
                }))
                .GroupBy(x => x.CategoryName)
                .Select(g => new BookCountByCategoryViewModel
                {
                    CategoryName = g.Key,
                    BookCount = g.Sum(x => x.Quantity) // Tổng số lượng sách bán
                })
                .ToList();

            // Lấy tất cả thể loại để đảm bảo kết quả trả về đủ (kể cả thể loại không bán được sách nào)
            var allCategories = await _categoryRepository.GetAllAsync();
            var result = allCategories
                .Select(c => new BookCountByCategoryViewModel
                {
                    CategoryName = c.CategoryName,
                    BookCount = categoryCounts.FirstOrDefault(cc => cc.CategoryName == c.CategoryName)?.BookCount ?? 0
                })
                .ToList();
            // In ra để kiểm tra
            foreach (var category in result)
            {
                Console.WriteLine($"Category: {category.CategoryName}, BookCount: {category.BookCount}");
            }
            return result;
        }

        //th?ng k� s�ch b�n ch?y nh?t
        public async Task<List<BookSaleInfoViewModel>> GetBestSellingBooksAsync(DateTime? start, DateTime? end, int topCount)
        {
            // in ra ngày để check
            Console.WriteLine($"Start: {start}, End: {end}, TopCount: {topCount}");
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (start.HasValue && end.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value).ToList();
            }
            //in ra để kiểm tra
            foreach (var order in orders)
            {
                Console.WriteLine($"OrderId: {order.OrderId}, CreatedAt: {order.CreatedAt}, Status: {order.Status}");
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
            //in ra để kiểm tra
            // foreach (var bookSale in bookSales)
            // {
            //     Console.WriteLine($"BookId: {bookSale.BookId}, Title: {bookSale.Title}, QuantitySold: {bookSale.QuantitySold}");
            // }
            var bookBestSelling = bookSales.OrderByDescending(bs => bs.QuantitySold).Take(topCount).ToList();
            //in ra để kiểm tra
            // foreach (var bookSale in bookBestSelling)
            // {
            //     Console.WriteLine($"Least Selling Book - BookId: {bookSale.BookId}, Title: {bookSale.Title}, QuantitySold: {bookSale.QuantitySold}");
            // }
            return bookBestSelling;
        }
        //th?ng k� s�ch �t b�n nh?t
        public async Task<List<BookSaleInfoViewModel>> GetLeastSellingBooksAsync(DateTime? start, DateTime? end, int topCount)
        {
            Console.WriteLine($"Start: {start}, End: {end}, TopCount: {topCount}");

            var orders = await _orderRepository.GetAllOrdersAsync();

            if (start.HasValue && end.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= start.Value && o.CreatedAt <= end.Value).ToList();
            }

            foreach (var order in orders)
            {
                Console.WriteLine($"OrderId: {order.OrderId}, CreatedAt: {order.CreatedAt}, Status: {order.Status}");
            }

            var books = await _bookRepository.GetAllAsync();

            if (books == null || !books.Any())
            {
                throw new Exception("No books found.");
            }

            var bookSales = books.Select(book =>
            {
                var quantitySold = orders.Sum(o => 
                    o.OrderDetails?.Where(od => od.BookId == book.BookId).Sum(od => od.Quantity) ?? 0
                );

                return new BookSaleInfoViewModel
                {
                    BookId = book.BookId,
                    Title = book.BookTitle,
                    QuantitySold = quantitySold
                };
            }).ToList();

            var bookLeastSelling = bookSales
                .OrderBy(bs => bs.QuantitySold)
                .ThenBy(bs => bs.Title)
                .Take(topCount)
                .ToList();

            return bookLeastSelling;
        }

        //th?ng k� s�ch ???c ?�nh gi� cao nh?t
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
        //th?ng k� s�ch b? ?�nh gi� th?p nh?t
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
        //th?ng k� t?ng s? ng??i d�ng
        public async Task<int> GetTotalUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                throw new Exception("No users found.");
            }
            return users.Count();
        }
        //th?ng k� ng??i d�ng m?i trong ng�y
        public async Task<int> GetNewUsersTodayAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                throw new Exception("No users found.");
            }
            return users.Count(u => u.CreatedUser.Date == DateTime.UtcNow.Date);
        }
        //th?ng k� ng??i d�ng m?i trong th�ng
        public async Task<int> GetNewUsersThisMonthAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                throw new Exception("No users found.");
            }
            return users.Count(u => u.CreatedUser.Year == DateTime.UtcNow.Year && u.CreatedUser.Month == DateTime.UtcNow.Month);
        }
        //th?ng k� ng??i d�ng c� ph�t sinh ??n h�ng
        public async Task<int> GetUsersWithOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (orders == null || !orders.Any())
            {
                throw new Exception("No orders found.");
            }
            return orders.Select(o => o.UserId).Distinct().Count();
        }
        //th?ng k� ng??i d�ng c� ?�nh gi� t�ch c?c
        public async Task<int> GetPositiveReviewUsersAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }
            return reviews.Where(r => r.Rating >= 4).Select(r => r.UserId).Distinct().Count();
        }
        //th?ng k� ng??i d�ng c� ?�nh gi� ti�u c?c
        public async Task<int> GetNegativeReviewUsersAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }
            return reviews.Where(r => r.Rating <= 2).Select(r => r.UserId).Distinct().Count();
        }
        //thong k� t?ng s? ?�nh gi�
        public async Task<int> GetTotalReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }
            return reviews.Count();
        }
        //th?ng k� ?�nh gi� trung b�nh
        public async Task<double> GetAverageRatingAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }
            return reviews.Average(r => r.Rating);
        }
        //th?ng k� t? l? ?�nh gi� t�ch c?c
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
        //th?ng k� t? l? ?�nh gi� ti�u c?c
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
        //th?ng k� s�ch c� nhi?u ?�nh gi� nh?t
        public async Task<MostReviewedBookViewModel?> GetMostReviewedBookAsync()
        {
            var reviews  = await _unitOfWork.Review.GetAllAsync("Book");
            if (reviews == null || !reviews.Any())
            {
                throw new Exception("No reviews found.");
            }

            var mostReviewedBook = reviews.GroupBy(r => r.BookId)
                .Select(g => new MostReviewedBookViewModel
                {
                    BookId = g.Key,
                    Image = g.First().Book?.ImageURL,
                    Title = g.First().Book?.BookTitle ?? "Unknown",
                    TotalReviews = g.Count(),
                    AverageRating = g.Average(r => r.Rating)
                }).OrderByDescending(mr => mr.TotalReviews).FirstOrDefault();

            return mostReviewedBook;
        }
    }
}