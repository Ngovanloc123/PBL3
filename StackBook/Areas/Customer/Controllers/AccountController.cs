using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackBook.Interfaces;

namespace StackBook.Areas.Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult Orders()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = Request.Cookies["userId"];


            var userProfile = await _userService.GetUserById(Guid.Parse(userId));
            if (userProfile == null)
            {
                return NotFound();
            }

            return View(userProfile.Data);
        }

        public IActionResult Vouchers()
        {
            return View();
        }




    }
}
