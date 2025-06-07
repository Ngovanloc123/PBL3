using System.ComponentModel.DataAnnotations;

namespace StackBook.ViewModels
{
    public class ChartVM
    {
        public List<string> Labels { get; set; }
        public List<double> Values { get; set; }

        public ChartVM(List<BookCountByCategoryViewModel> bookCategories)
        {
            if (bookCategories != null && bookCategories.Any())
            {
                Labels = bookCategories.Select(bc => bc.CategoryName).ToList();
                Values = bookCategories.Select(bc => (double)bc.BookCount).ToList();
            }
        }

        public ChartVM(List<RevenueByCategoryViewModel> bookCategories)
        {
            if (bookCategories != null && bookCategories.Any())
            {
                Labels = bookCategories.Select(bc => bc.CategoryName).ToList();
                Values = bookCategories.Select(bc => bc.Revenue).ToList();
            }
        }

        public ChartVM(List<RevenueByDateViewModel> revenueByDate)
        {
            if (revenueByDate != null && revenueByDate.Any())
            {
                Labels = revenueByDate.Select(r => r.Date.ToString("dd/MM")).ToList();
                Values = revenueByDate.Select(r => r.Revenue).ToList();
            }
        }
        public ChartVM(List<BookSaleInfoViewModel> bookSale)
        {
            if (bookSale != null && bookSale.Any())
            {
                Labels = bookSale.Select(b => b.Title).ToList();
                Values = bookSale.Select(r => (double)r.QuantitySold).ToList();
            }
        }

        // Default constructor  
        public ChartVM() { }

    }

    public class BookChart
    {
        public ChartVM CategoryCurrentMonth { get; set; }
        public ChartVM CategoryLastMonth { get; set; }
        public ChartVM BooksSale { get; set; }

    }
    public class OrderStatisticsVM
    {
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int CanceledOrders { get; set; }
        public int ReturnedOrders { get; set; }
        public int ShippingOrders { get; set; }
        public double TotalRevenue { get; set; }
        public double CancelRate { get; set; }
        public List<TopCustomerViewModel> TopCustomers { get; set; } = new List<TopCustomerViewModel>();
        public ChartVM RevenueByDate { get; set; } = new ChartVM();
        public ChartVM RevenueByCategory { get; set; } = new ChartVM();
        public ChartVM OrderStatusChart { get; set; } = new ChartVM();

        [Display(Name = "From date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "To date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Time type")]
        public TimeRangeType TimeRangeType { get; set; } = TimeRangeType.Daily;
    }

    public class BookStatisticsVM
    {
        public List<BookSaleInfoViewModel> BestSellingBooks { get; set; } = new List<BookSaleInfoViewModel>();
        public List<BookSaleInfoViewModel> LeastSellingBooks { get; set; } = new List<BookSaleInfoViewModel>();
        public List<BookRatingInfoViewModel> HighestRatedBooks { get; set; } = new List<BookRatingInfoViewModel>();
        public List<BookRatingInfoViewModel> LowestRatedBooks { get; set; } = new List<BookRatingInfoViewModel>();
        public ChartVM BookCountByCategory { get; set; } = new ChartVM();

        [Display(Name = "From date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "To date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Number of displays")]
        public int TopCount { get; set; } = 5;
    }

    public class UserStatisticsVM
    {
        public int TotalUsers { get; set; }
        public int NewUsersToday { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int UsersWithOrders { get; set; }
        public int PositiveReviewUsers { get; set; }
        public int NegativeReviewUsers { get; set; }
        //public ChartVM UserGrowthChart { get; set; } = new ChartVM();
        //public ChartVM UserTypeChart { get; set; } = new ChartVM();
    }

    public class ReviewStatisticsVM
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public double PositiveReviewRatePercent { get; set; }
        public double NegativeReviewRatePercent { get; set; }
        public MostReviewedBookViewModel? MostReviewedBook { get; set; }
    }
}