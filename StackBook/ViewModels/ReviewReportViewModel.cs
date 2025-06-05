using System;

namespace StackBook.ViewModels
{
    public class ReviewReportViewModel
    {
        // Tổng số lượt đánh giá
        public int TotalReviews { get; set; }

        // Trung bình điểm đánh giá (ví dụ: từ 1.0 đến 5.0)
        public double AverageRating { get; set; }

        // Tỷ lệ đánh giá tích cực (ví dụ: >= 4 sao) theo phần trăm
        public double PositiveReviewRatePercent { get; set; }

        // Tỷ lệ đánh giá tiêu cực (ví dụ: <= 2 sao) theo phần trăm
        public double NegativeReviewRatePercent { get; set; }

        // Sách có nhiều đánh giá nhất
        public MostReviewedBookViewModel? MostReviewedBook { get; set; }
    }

    public class MostReviewedBookViewModel
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
    }
}
