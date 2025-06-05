using System;

namespace StackBook.ViewModels
{
    public class UserReportViewModel
    {
        // Tổng số người dùng
        public int TotalUsers { get; set; }

        // Người dùng mới trong ngày
        public int NewUsersToday { get; set; }

        // Người dùng mới trong tháng
        public int NewUsersThisMonth { get; set; }

        // Người dùng có phát sinh đơn hàng
        public int UsersWithOrders { get; set; }

        // Người dùng có đánh giá tích cực (ví dụ: >= 4 sao)
        public int PositiveReviewUsers { get; set; }

        // Người dùng có đánh giá tiêu cực (ví dụ: <= 2 sao)
        public int NegativeReviewUsers { get; set; }
    }
}
