using System;
using System.Collections.Generic;

namespace StackBook.ViewModels
{
    public class BookReportViewModel
    {
        // Số lượng sách theo thể loại
        public List<BookCountByCategoryViewModel> BookCountByCategory { get; set; } = new List<BookCountByCategoryViewModel>();

        // Sách bán chạy nhất
        public List<BookSaleInfoViewModel> BestSellingBooks { get; set; } = new List<BookSaleInfoViewModel>();

        // Sách ít bán nhất
        public List<BookSaleInfoViewModel> LeastSellingBooks { get; set; } = new List<BookSaleInfoViewModel>();

        // Sách được đánh giá cao nhất
        public List<BookRatingInfoViewModel> HighestRatedBooks { get; set; } = new List<BookRatingInfoViewModel>();

        // Sách bị đánh giá thấp nhất
        public List<BookRatingInfoViewModel> LowestRatedBooks { get; set; } = new List<BookRatingInfoViewModel>();
    }

    public class BookCountByCategoryViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public int BookCount { get; set; }
    }

    public class BookSaleInfoViewModel
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
    }

    public class BookRatingInfoViewModel
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}
