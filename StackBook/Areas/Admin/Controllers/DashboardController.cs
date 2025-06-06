using Microsoft.AspNetCore.Mvc;
using StackBook.Interfaces;
using StackBook.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace StackBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(Roles = "Admin")]
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
                // Ch?nh l?i khi x�c nh?n hang th�nh c�ng => 5. th�nh c�ng
                YearlyRevenue = await _dashboardService.GetYearlyRevenueByStatusAsync(1, 2025)
            };

            return View(data);
        }
    }
}
