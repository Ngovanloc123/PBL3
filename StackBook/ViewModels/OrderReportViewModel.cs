using StackBook.Models;
using System;
using System.Collections.Generic;

namespace StackBook.ViewModels
{
    public class OrderReportViewModel
    {
        // ==== Khoảng thời gian thống kê ====
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeRangeType TimeRange { get; set; } = TimeRangeType.Custom;

        // ==== Tổng quan đơn hàng theo trạng thái ====
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int CanceledOrders { get; set; }
        public int ReturnedOrders { get; set; }
        public int ShippingOrders { get; set; }

        // ==== Doanh thu tổng ====
        public double TotalRevenue { get; set; }

        // ==== Doanh thu theo ngày/tuần/tháng ====
        public List<RevenueByDateViewModel> RevenueByDate { get; set; } = new List<RevenueByDateViewModel>();

        // ==== Tỷ lệ hủy đơn hàng (0 - 100%) ====
        public double CancelRatePercent { get; set; }

        // ==== Top khách hàng mua nhiều nhất ====
        public List<TopCustomerViewModel> TopCustomers { get; set; } = new List<TopCustomerViewModel>();

        // ==== Doanh thu theo thể loại sách ====
        public List<RevenueByCategoryViewModel> RevenueByCategory { get; set; } = new List<RevenueByCategoryViewModel>();
    }

    public class RevenueByDateViewModel
    {
        public DateTime Date { get; set; } // Có thể là ngày, tuần, hoặc tháng
        public double Revenue { get; set; }
    }

    public class TopCustomerViewModel
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public double TotalSpent { get; set; }
    }

    public class RevenueByCategoryViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public double Revenue { get; set; }
    }

    public enum TimeRangeType
    {
        Daily,
        Weekly,
        Monthly,
        Custom
    }
}
