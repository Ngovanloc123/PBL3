using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StackBook.Areas.Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    [Area("Customer")]
    public class AccountController : Controller
    {
        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult Orders()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult Vouchers()
        {
            return View();
        }


    }
}
