using DocumentFormat.OpenXml.Bibliography;
using StackBook.DAL.IRepository;
using StackBook.DAL.Repository;
using StackBook.Interfaces;
using StackBook.Models;

namespace StackBook.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderRepository _orderHistoryRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        public DashboardService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IOrderRepository orderHistoryRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _orderHistoryRepository = orderHistoryRepository;
        }

        public async Task<List<double>> GetYearlyRevenueByStatusAsync(int status, int year)
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync("OrderHistories");
                var monthlyRevenue = Enumerable.Repeat(0.0, 12).ToList(); // Khởi tạo 12 tháng với giá trị 0

                for (int month = 1; month <= 12; month++)
                {
                    monthlyRevenue[month - 1] = orders
                        .Where(o => o.OrderHistories.Any(oh =>
                            oh.Status == status &&
                            oh.createdStatus.Month == month &&
                            oh.createdStatus.Year == year))
                        .Sum(o => o.TotalPrice);
                }

                return monthlyRevenue;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy doanh thu 12 tháng năm {year} theo status {status}: {ex.Message}");
            }
        }

    }
}
