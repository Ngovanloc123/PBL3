using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackBook.ViewModels;

namespace StackBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public class StatisticController : Controller
    {
        
        public IActionResult Index()
        {
            var chartData = new ChartVM
            {
                Labels = new List<string> { "Tháng 1", "Tháng 2", "Tháng 3" },
                Values = new List<int> { 100, 150, 120 }
            };

            return View(chartData);
        }
    }
}