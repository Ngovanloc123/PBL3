using Microsoft.AspNetCore.Mvc;
namespace StackBook.Areas.Account.Controllers
{
    [Area("Site")]
    [Route("Site/[controller]/[action]")]  // Chỉ định rõ đường dẫn trong area
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

        [HttpGet("Profile/{userId}")]
        public IActionResult Profile(Guid userId)
        {
            return View();
        }
    }
}

