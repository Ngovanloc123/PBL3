using Microsoft.AspNetCore.Mvc;

namespace StackBook.Areas.Account.Controllers
{
    [Area("Site")]
    public class AccountController : Controller
    {
        public IActionResult Signin()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}
