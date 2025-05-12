using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StackBook.Areas.Customer.Controllers
{
    public class OrderController : Controller
    {
        [Authorize(Roles = "Customer")]
        [Area("Customer")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BuyNow(Guid bookId)
        {
            // Logic xử lý mua ngay
            TempData["Message"] = "Proceeding to checkout!";
            return RedirectToAction("Checkout", "Order");
        }
    }
}
