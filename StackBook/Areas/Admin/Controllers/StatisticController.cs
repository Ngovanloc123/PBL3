using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackBook.Interfaces;
using StackBook.ViewModels;

namespace StackBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public class StatisticController : Controller
    {
        private readonly IReportService _reportService;

        public StatisticController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            DateTime now = DateTime.Now;

            DateTime startOfCurrentMonth = new DateTime(now.Year, now.Month, 1);
            DateTime endOfCurrentMonth = startOfCurrentMonth.AddMonths(1).AddDays(-1);

            DateTime startOfLastMonth = startOfCurrentMonth.AddMonths(-1);
            DateTime endOfLastMonth = startOfCurrentMonth.AddDays(-1);


            // Tháng hiện tại
            List<BookCountByCategoryViewModel> bookCountByCategory = await _reportService.GetBookCountByCategoryAsync(startOfCurrentMonth, endOfCurrentMonth);
            var categoryCurrentMonth = new ChartVM(bookCountByCategory);

            // Tháng hiện tại
            bookCountByCategory = await _reportService.GetBookCountByCategoryAsync(startOfLastMonth, endOfLastMonth);
            var categoryLastMonth = new ChartVM(bookCountByCategory);

            var booksSaleRaw = await _reportService.GetBestSellingBooksAsync(startOfCurrentMonth, endOfCurrentMonth);
            var booksSale = new ChartVM(booksSaleRaw);

            var data = new BookChart
            {
                CategoryLastMonth = categoryLastMonth,
                CategoryCurrentMonth = categoryCurrentMonth,
                BooksSale = booksSale
            };


            return View(data);
        }

        public async Task<IActionResult> Orders(DateTime? startDate, DateTime? endDate, TimeRangeType timeRangeType = TimeRangeType.Daily)
        {
            // Thiết lập ngày mặc định nếu không có
            if (!startDate.HasValue || !endDate.HasValue)
            {
                endDate = DateTime.Now.Date;
                startDate = endDate.Value.AddDays(-30);
            }

            var model = new OrderStatisticsVM
            {
                StartDate = startDate,
                EndDate = endDate,
                TimeRangeType = timeRangeType
            };

            try
            {
                // Thống kê tổng quan đơn hàng
                model.TotalOrders = await _reportService.GetTotalOrdersAsync(startDate, endDate);
                model.CompletedOrders = (await _reportService.GetCompletedOrdersAsync(startDate, endDate)).Count;
                model.ProcessingOrders = (await _reportService.GetProcessingOrdersAsync(startDate, endDate)).Count;
                model.CanceledOrders = (await _reportService.GetCanceledOrdersAsync(startDate, endDate)).Count;
                model.ReturnedOrders = (await _reportService.GetReturnedOrdersAsync(startDate, endDate)).Count;
                model.ShippingOrders = (await _reportService.GetShippingOrdersAsync(startDate, endDate)).Count;

                // Doanh thu và tỷ lệ
                model.TotalRevenue = await _reportService.GetTotalRevenueAsync(startDate, endDate);
                model.CancelRate = await _reportService.GetCancelRateAsync(startDate, endDate);

                // Top khách hàng
                model.TopCustomers = await _reportService.GetTopCustomersAsync(startDate, endDate, 10);

                // Biểu đồ doanh thu theo thời gian
                var revenueByDate = await _reportService.GetRevenueByDateAsync(startDate, endDate, timeRangeType);
                model.RevenueByDate = new ChartVM(revenueByDate);

                // Biểu đồ doanh thu theo danh mục
                var revenueByCategory = await _reportService.GetRevenueByCategoryAsync(startDate, endDate);
                model.RevenueByCategory = new ChartVM(revenueByCategory);

                // Biểu đồ trạng thái đơn hàng
                model.OrderStatusChart = new ChartVM
                {
                    Labels = new List<string> { "Delivered", "Pending", "Canceled", "Returned", "Shipping" },
                    Values = new List<double> { model.CompletedOrders, model.ProcessingOrders, model.CanceledOrders, model.ReturnedOrders, model.ShippingOrders }
                };
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading order statistics: " + ex.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> Books(DateTime? startDate, DateTime? endDate, int topCount = 5)
        {
            // Thiết lập ngày mặc định nếu không có
            if (!startDate.HasValue || !endDate.HasValue)
            {
                endDate = DateTime.Now.Date;
                startDate = endDate.Value.AddDays(-30);
            }

            var data = new BookStatisticsVM
            {
                StartDate = startDate,
                EndDate = endDate,
                TopCount = topCount
            };

            try
            {
                Console.WriteLine($"StartDate: {startDate}, EndDate: {endDate}, TopCount: {topCount}");
                data.BestSellingBooks = await _reportService.GetBestSellingBooksAsync(startDate, endDate, topCount);
                //in ra để debug
                foreach (var book in data.BestSellingBooks)
                {
                    Console.WriteLine($"BookId: {book.BookId}, Title: {book.Title}, TotalSold: {book.QuantitySold}");
                }
                data.LeastSellingBooks = await _reportService.GetLeastSellingBooksAsync(startDate, endDate, topCount);

                data.HighestRatedBooks = await _reportService.GetHighestRatedBooksAsync(startDate, endDate, topCount);
                data.LowestRatedBooks = await _reportService.GetLowestRatedBooksAsync(startDate, endDate, topCount);

                var bookCountByCategory = await _reportService.GetBookCountByCategoryAsync(startDate, endDate);
                data.BookCountByCategory = new ChartVM(bookCountByCategory);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading book statistics: " + ex.Message;
            }

            return View(data);
        }

        public async Task<IActionResult> Users()
        {
            try
            {
                var data = new UserStatisticsVM
                {
                    TotalUsers = await _reportService.GetTotalUsersAsync(),
                    NewUsersToday = await _reportService.GetNewUsersTodayAsync(),
                    NewUsersThisMonth = await _reportService.GetNewUsersThisMonthAsync(),
                    UsersWithOrders = await _reportService.GetUsersWithOrdersAsync(),
                    PositiveReviewUsers = await _reportService.GetPositiveReviewUsersAsync(),
                    NegativeReviewUsers = await _reportService.GetNegativeReviewUsersAsync()
                };

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading users statistics: " + ex.Message;

                var emptyData = new UserStatisticsVM();
                return View(emptyData);
            }
        }

        public async Task<IActionResult> Reviews()
        {
            var data = new ReviewStatisticsVM();

            try
            {
                data.TotalReviews = await _reportService.GetTotalReviewsAsync();
                data.AverageRating = await _reportService.GetAverageRatingAsync();
                data.PositiveReviewRatePercent = await _reportService.GetPositiveReviewRatePercentAsync();
                data.NegativeReviewRatePercent = await _reportService.GetNegativeReviewRatePercentAsync();
                data.MostReviewedBook = await _reportService.GetMostReviewedBookAsync();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading review statistics: " + ex.Message;
            }

            return View(data);
        }
    }
}

