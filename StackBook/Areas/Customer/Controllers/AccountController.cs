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
    //[Authorize]
    [Route("Customer/Account")] // Route cơ bản
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly JwtUtils _jwtUtils;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CloudinaryUtils _cloudinaryUtils;
        private readonly IDiscountService _discountService;
        private readonly INotificationService _notificationService;

        public AccountController(IUserService userService, IAuthService authService, IDiscountService discountService, INotificationService notificationService, IHttpContextAccessor httpContextAccessor, JwtUtils jwtUtils, CloudinaryUtils cloudinaryUtils)
        {
            _userService = userService;
            _authService = authService;
            _discountService = discountService;
            _notificationService = notificationService;
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
            if(userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });
            //Xem cookie access token có tồn tại hay không
            Console.WriteLine($"AccessToken: {_httpContextAccessor.HttpContext?.Request.Cookies["accessToken"]}");
            //Xem cookie refresh token có tồn tại hay không
            Console.WriteLine($"RefreshToken: {_httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"]}");
            //Sem user qua cookie
            var user = _userService.GetUserById(Guid.Parse(userIdValue));
            if(user == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User not found.", StatusCode = 404 });
            if(user.Result.Data == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User not found.", StatusCode = 404 });
            var data = user.Result.Data.RefreshToken;
            Console.WriteLine($"RefreshToken: {data}");
            Console.WriteLine($"UserIdValue: {userIdValue}");
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found.", StatusCode = 404 });
            Guid userId = Guid.Parse(userIdValue);
            var response = await _userService.GetUserById(userId);
            if (response == null) return NotFound();
            return View(response.Data);
        }

        [HttpPost("Update-Avatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return View("Error", new ErrorViewModel { ErrorMessage = "Image file is required.", StatusCode = 400 });
            var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found.", StatusCode = 404 });
            Guid userId = Guid.Parse(userIdValue);
            var response = await _userService.UpdateAvatar(userId, image);
            var user = await _userService.GetUserById(userId);
            if (user.Data == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User not found.", StatusCode = 404 });    
            var token = _jwtUtils.GenerateAccessToken(user.Data);
            
            Response.Cookies.Append("accessToken", token, new CookieOptions
            {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            });
            if (response.Success)
            {
                //Tạo thông báo cho người dùng
                await _notificationService.SendNotificationAsync(userId, "Your avatar has been updated successfully.");
                // Trả về trang Profile sau khi cập nhật thành công
                return RedirectToAction("Profile");
            }
            else
            {
                return View("Error", new ErrorViewModel { ErrorMessage = response.Message, StatusCode = 400 });
            }
            
        }
        [HttpPost("Update-Username")]
        public async Task<IActionResult> UpdateUsername(string username)
        {
            Console.WriteLine($"UpdateUsername Username: {username}");
            if (string.IsNullOrEmpty(username))
                return View("Error", new ErrorViewModel { ErrorMessage = "Username is required.", StatusCode = 400 });
            var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found.", StatusCode = 404 });
            Guid userId = Guid.Parse(userIdValue);
            var response = await _userService.UpdateUsername(userId, username);
            //cap nhat lai access token vi username da bi thay doi
            var user = await _userService.GetUserById(userId);
            if(user.Data == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User not found.", StatusCode = 404 });
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
                //Tạo thông báo cho người dùng
                await _notificationService.SendNotificationAsync(userId, "Your username has been updated successfully.");
                // Trả về trang Profile sau khi cập nhật thành công
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
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found.", StatusCode = 404 });
            Guid userId = Guid.Parse(userIdValue);
            var response = await _userService.UpdateEmail(userId, email);
            if (response.Success)
            {
                //logout 
                //Tạo thông báo cho người dùng
                await _notificationService.SendNotificationAsync(userId, "Your email has been updated successfully. Please log in again.");
                return RedirectToAction("SignOut", "Account", new { area = "Site" });
            }
            else
            {
                return View("Error", new ErrorViewModel { ErrorMessage = response.Message, StatusCode = response.StatusCode });
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
                //Tạo thông báo cho người dùng
                await _notificationService.SendNotificationAsync(userId, "Your password has been changed successfully. Please log in again.");
                return RedirectToAction("SignOut", "Account", new { area = "Site" });
            }
            else
            {
                return View("Error", new ErrorViewModel { ErrorMessage = response.Message });
            }
        }
        [HttpGet("Notifications")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Notifications()
        {
            //Xem tất cả thông báo của người dùng
            var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });
            Guid userId = Guid.Parse(userIdValue);
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            if (notifications == null || notifications.Count == 0)
            {
                ViewBag.Message = "You have no notifications.";
                return View(new List<Notification>());
            }
            return View(notifications);
        }
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userIdCookie = Request.Cookies["userId"];
            Console.WriteLine($"UserId Cookie: {userIdCookie}");
            if (string.IsNullOrEmpty(userIdCookie))
            {
                return Unauthorized();
            }
            if (!Guid.TryParse(userIdCookie, out var userId))
            {
                return BadRequest("UserId không hợp lệ");
            }
            // Lấy notification bằng id đúng
            Console.WriteLine($"Notification ID: {id}");
            var notification = await _notificationService.GetNotificationByIdAsync(id);

            if (notification == null || notification.UserId != userId)
            {
                return Forbid();
            }

            await _notificationService.MarkAsReadAsync(id);

            return RedirectToAction("Notifications");
        }


        [HttpGet("WishList")]
        [Authorize(Roles = "User")]
        public IActionResult WishList()
        {
            var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });
            //Xem cookie access token có tồn tại hay không
            Console.WriteLine($"AccessToken: {_httpContextAccessor.HttpContext?.Request.Cookies["accessToken"]}");
            //Xem cookie refresh token có tồn tại hay không
            Console.WriteLine($"RefreshToken: {_httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"]}");
            //Sem user qua cookie
            var user = _userService.GetUserById(Guid.Parse(userIdValue));
            if (user == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User not found.", StatusCode = 404 });
            if (user.Result.Data == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User not found.", StatusCode = 404 });

            return View(user.Result.Data);
        }
        [HttpGet("Vouchers")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Vouchers()
        {
            // Lấy tất cả mã giảm giá từ dịch vụ check ngày hệ thống
            var discounts = await _discountService.GetActiveDiscounts(DateTime.Now);
            // Kiểm tra nếu không có mã giảm giá nào
            if (discounts == null || discounts.Count == 0)
            {
                // Trả về thông báo không có mã giảm giá
                ViewBag.Message = "Hiện tại không có mã giảm giá nào.";
                return View(new List<Discount>());
            }
            return View(discounts);
        }
    }
}
