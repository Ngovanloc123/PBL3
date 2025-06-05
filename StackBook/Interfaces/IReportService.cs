using StackBook.Models;
using StackBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.Interfaces
{
    public interface IReportService
    {
        Task<int> GetTotalOrdersAsync(DateTime? start, DateTime? end);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetOrdersByStatusAsync(int status, DateTime? start, DateTime? end);
        Task<List<Order>> GetCompletedOrdersAsync(DateTime? start, DateTime? end);
        Task<List<Order>> GetProcessingOrdersAsync(DateTime? start, DateTime? end);
        Task<List<Order>> GetCanceledOrdersAsync(DateTime? start, DateTime? end);
        Task<List<Order>> GetReturnedOrdersAsync(DateTime? start, DateTime? end);
        Task<List<Order>> GetShippingOrdersAsync(DateTime? start, DateTime? end);
        Task<double> GetTotalRevenueAsync(DateTime? start, DateTime? end);
        Task<List<RevenueByDateViewModel>> GetRevenueByDateAsync(DateTime? start, DateTime? end, TimeRangeType rangeType);
        Task<double> GetCancelRateAsync(DateTime? start, DateTime? end);
        Task<List<TopCustomerViewModel>> GetTopCustomersAsync(DateTime? start, DateTime? end, int top = 5);
        Task<List<RevenueByCategoryViewModel>> GetRevenueByCategoryAsync(DateTime? start, DateTime? end);
        Task<List<BookCountByCategoryViewModel>> GetBookCountByCategoryAsync(DateTime? start, DateTime? end);
        Task<List<BookSaleInfoViewModel>> GetBestSellingBooksAsync(DateTime? start, DateTime? end, int top = 5);
        Task<List<BookSaleInfoViewModel>> GetLeastSellingBooksAsync(DateTime? start, DateTime? end, int top = 5);
        Task<List<BookRatingInfoViewModel>> GetHighestRatedBooksAsync(DateTime? start, DateTime? end, int top = 5);
        Task<List<BookRatingInfoViewModel>> GetLowestRatedBooksAsync(DateTime? start, DateTime? end, int top = 5);
        Task<int> GetTotalUsersAsync();
        Task<int> GetNewUsersTodayAsync();
        Task<int> GetNewUsersThisMonthAsync();
        Task<int> GetUsersWithOrdersAsync();
        Task<int> GetPositiveReviewUsersAsync();
        Task<int> GetNegativeReviewUsersAsync();
        Task<int> GetTotalReviewsAsync();
        Task<double> GetAverageRatingAsync();
        Task<double> GetPositiveReviewRatePercentAsync();
        Task<double> GetNegativeReviewRatePercentAsync();
        Task<MostReviewedBookViewModel?> GetMostReviewedBookAsync();
    }
}