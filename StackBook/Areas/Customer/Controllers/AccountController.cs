using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Utils;
using StackBook.DTOs;
using System.Security.Claims;

namespace StackBook.Areas.Customer.Controllers 
{
    [Area("Customer")]
    [Authorize]
    [Route("Customer/Account")] // Route cơ bản
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly JwtUtils _jwtUtils;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CloudinaryUtils _cloudinaryUtils;

        public AccountController(IUserService userService, IAuthService authService, IHttpContextAccessor httpContextAccessor, JwtUtils jwtUtils, CloudinaryUtils cloudinaryUtils)
        {
            _userService = userService;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _jwtUtils = jwtUtils;
            _cloudinaryUtils = cloudinaryUtils;
        }
        [HttpGet("Profile")]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> Profile()
        {
            //User Id của người dùng hiện tại trong cookie
            var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Xem cookie access token có tồn tại hay không
            Console.WriteLine($"AccessToken: {_httpContextAccessor.HttpContext?.Request.Cookies["accessToken"]}");
            //Xem cookie refresh token có tồn tại hay không
            Console.WriteLine($"RefreshToken: {_httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"]}");
            //Sem user qua cookie
            var user = _userService.GetUserById(Guid.Parse(userIdValue));
            var data = user.Result.Data.RefreshToken;
            Console.WriteLine($"RefreshToken: {data}");
            Console.WriteLine($"UserIdValue: {userIdValue}");
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });
            Guid userId = Guid.Parse(userIdValue);
            var response = await _userService.GetUserById(userId);
            if (response == null) return NotFound();
            return View(response.Data);
        }
        [HttpPost("UpdateAvatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return View("Error", new ErrorViewModel { ErrorMessage = "Image file is required." });
            var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });
            Guid userId = Guid.Parse(userIdValue);
            var response = await _userService.UpdateAvatar(userId, image);
            if (response.Success)
            {
                return RedirectToAction("Profile");
            }
            else
            {
                return View("Error", new ErrorViewModel { ErrorMessage = response.Message });
            }
        }
        [HttpPost("Update-Username")]
        public async Task<IActionResult> UpdateUsername(string username)
        {
            Console.WriteLine($"UpdateUsername Username: {username}");
            if (string.IsNullOrEmpty(username))
                return View("Error", new ErrorViewModel { ErrorMessage = "Username is required." });
            var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });
            Guid userId = Guid.Parse(userIdValue);
            var response = await _userService.UpdateUsername(userId, username);
            //xoa cookie access token
            Response.Cookies.Delete("accessToken");
            //cap nhat lai access token vi username da bi thay doi
            var user = await _userService.GetUserById(userId);
            var token = _jwtUtils.GenerateAccessToken(user.Data);
            response.AccessToken = token;
            //cap nhat lai cookie access token
            Response.Cookies.Append("accessToken", response.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            });
            if (response.Success)
            {
                return RedirectToAction("Profile");
            }
            else
            {
                return View("Error", new ErrorViewModel { ErrorMessage = response.Message });
            }
        }
        [HttpPost("Update-Email")]
        public async Task<IActionResult> UpdateEmail(string email)
        {
            Console.WriteLine($"UpdateEmail Email: {email}");
            if (string.IsNullOrEmpty(email))
                return View("Error", new ErrorViewModel { ErrorMessage = "Email is required." });
            var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });
            Guid userId = Guid.Parse(userIdValue);
            var response = await _userService.UpdateEmail(userId, email);
            if (response.Success)
            {
                //logout 
                return RedirectToAction("SignOut", "Account", new { area = "Site" });
            }
            else
            {
                return View("Error", new ErrorViewModel { ErrorMessage = response.Message });
            }
        }
        [HttpPost("Change-Password")]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            Console.WriteLine(currentPassword);
            Console.WriteLine(newPassword);
            Console.WriteLine(confirmPassword);
            if(string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Email is required." }); 
            }
             var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });
            Guid userId = Guid.Parse(userIdValue);
            Console.WriteLine(userId);
            if(!newPassword.Equals(confirmPassword))
                return View("Error", new ErrorViewModel { ErrorMessage = "Not Map" });
            var response = await _userService.UpdatePassword(userId, currentPassword, newPassword);
            if (response.Success)
            {
                return RedirectToAction("SignOut", "Account", new { area = "Site" });
            }
            else
            {
                return View("Error", new ErrorViewModel { ErrorMessage = response.Message });
            }
        }
        [HttpGet("Notifications")]
         public IActionResult Notifications()
        {
            return View();
        }
        [HttpGet("Orders")]
         public IActionResult Orders()
        {
            return View();
        }
        [HttpGet("WishList")]
         public IActionResult WishList()
        {
            return View();
        }
        [HttpGet("Vouchers")]
        [Authorize(Roles = "User")]
        public IActionResult Vouchers()
        {
            return View();
        }
    }
}
