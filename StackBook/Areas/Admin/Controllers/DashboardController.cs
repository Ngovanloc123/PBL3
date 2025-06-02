using Microsoft.AspNetCore.Mvc;
using StackBook.Interfaces;
using StackBook.ViewModels;

namespace StackBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            

            var data = new DashboardVM
            {
                // Ch?nh l?i khi xác nh?n hang thành công => 5. thành công
                YearlyRevenue = await _dashboardService.GetYearlyRevenueByStatusAsync(1, 2025)
            };

            return View(data);
        }
    }
}
