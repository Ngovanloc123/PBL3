using Microsoft.AspNetCore.Mvc;
using StackBook.DTOs;
using StackBook.Services;
using StackBook.Models;
using System.Threading.Tasks;
using StackBook.Middleware;
using Microsoft.AspNetCore.Authorization;
using StackBook.ViewModels;
using StackBook.Interfaces;
using StackBook.Utils;

namespace StackBook.Controllers
{
    [Route("Auth/[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly JwtUtils _jwtUtils;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IUserService userService, IAuthService authService, IHttpContextAccessor httpContextAccessor, JwtUtils jwtUtils)
        {
            _userService = userService;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _jwtUtils = jwtUtils;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(UserVM.RegisterVM registerDto)
        {
            try
            {
                if (registerDto == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });

                var result = await _authService.RegisterUser(registerDto);
                if (result == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Registration failed." });

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View();
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInUser(UserVM.SignInVM signInDto)
        {
            try
            {
                if (signInDto == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });

                var result = await _authService.SignInUser(signInDto);
                if (result.Success == false)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Login failed." });

                // Ghi access token vào cookie
                Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                });

                // Ghi refresh token vào cookie
                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                // Nếu muốn: Ghi thêm user ID (không bắt buộc)
                Response.Cookies.Append("userId", result.Data.UserId.ToString(), new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                return RedirectToAction("Profile", new { id = result.Data?.UserId });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPut("Update/{userId}")]
        [Authorize]  // Đảm bảo chỉ người dùng đã xác thực mới có quyền truy cập
        public async Task<IActionResult> UpdateUser(Guid userId, UpdateDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });

                // Lấy userId từ claims trong token (cookie)
                var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                var userRoleClaims = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                if (currentUserIdClaims == null)
                    return Unauthorized("User not authenticated.");

                var currentUserId = Guid.Parse(currentUserIdClaims);

                // Kiểm tra xem người dùng có quyền sửa thông tin hay không
                if (userId != currentUserId) 
                // Nếu không phải người dùng tự chỉnh sửa thì kiểm tra vai trò
                {
                    if (userRoleClaims != "Admin")
                    {
                        return View("Error", new ErrorViewModel { ErrorMessage = "You are not authorized to edit other users' profiles." });
                    }

                    // Nếu là Admin, nhưng Admin không thể chỉnh sửa thông tin của Admin khác
                    if (userRoleClaims == "Admin" && userId == currentUserId)
                    {
                        return View("Error", new ErrorViewModel { ErrorMessage = "Admin cannot modify their own profile here." });
                    }
                }

                updateDto.UserId = userId;
                var result = await _userService.UpdateUser(updateDto);
                // Cập nhật lại accessToken sau khi thay đổi
                // Lưu lại accessToken vào cookie (giống như trước khi đăng nhập)
                Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                });

                return RedirectToAction("Profile", new { id = result.Data?.UserId });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }


        // Phương thức đăng xuất
        [HttpGet("Logout")]
        [Authorize]  // Đảm bảo người dùng đã xác thực
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Lấy userId từ claim trong token (cookie)
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }

                // Đăng xuất người dùng (thực hiện các hành động cần thiết từ phía server, ví dụ: hủy session, xóa refresh token...)
                await _authService.LogoutUser(Guid.Parse(userId));

                // Xóa cookie chứa access token khi người dùng đăng xuất
                Response.Cookies.Delete("accessToken");  // Xóa accessToken khỏi cookie
                Response.Cookies.Delete("refreshToken"); // Xóa refreshToken khỏi cookie (nếu có)

                // Chuyển hướng về trang login
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }


        [HttpGet("google-login")]
        public async Task<IActionResult> LoginWithGoogle(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });

                var result = await _authService.LoginWithGoogle(code);
                if (result == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Login failed." });

                return RedirectToAction("Profile", new { id = result.Data?.UserId, accessToken = result.AccessToken, refreshToken = result.RefreshToken });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });

                var result = await _authService.LoginWithGoogle(code);
                if (result == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Login failed." });

                return RedirectToAction("Profile", new { id = result.Data?.UserId, accessToken = result.AccessToken, refreshToken = result.RefreshToken });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
    }
}
