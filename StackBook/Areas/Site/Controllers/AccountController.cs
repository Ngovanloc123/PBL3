using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Utils;
using StackBook.DTOs;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using StackBook.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using static StackBook.ViewModels.UserVM;

namespace StackBook.Areas.Site.Controllers 
{
    [Area("Site")]
    [Route("Site/Account")] // Route cơ bản
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

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInUser(SignInVM signInVM)
        {
            try
            {
                if (signInVM == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data.", StatusCode = 400 });

                var result = await _authService.SignInUser(signInVM);
                if (result.Success == false)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Login failed.", StatusCode = result.StatusCode });
                // Giữ JWT token trong cookie để sử dụng cho API calls
                if(result.AccessToken == null || result.RefreshToken == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Login failed.", StatusCode = 400 });
                Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10)
                });
                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });
                // Ghi thêm userId (không bắt buộc)
                if(result.Data == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Login failed.", StatusCode = 400 });
                if (result.Data.UserId == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Login failed.", StatusCode = 400 });
                Response.Cookies.Append("userId", result.Data.UserId.ToString(), new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                var tokenHandler = _jwtUtils.ValidateToken(result.AccessToken);
                if (tokenHandler == null)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token.", StatusCode = 401 });
                }

                var claimsIdentity = tokenHandler.Identity as ClaimsIdentity;
                if (claimsIdentity == null)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token.", StatusCode = 401 });
                }

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                };

                //
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Kiểm tra vai trò
                var roleClaim = claimsIdentity.FindFirst(ClaimTypes.Role);
                if (roleClaim == null)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Role claim not found.", StatusCode = 401 });
                }

                if (roleClaim.Value == "Admin")
                {
                    TempData["success"] = "Sign in successful.";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else if (roleClaim.Value == "User")
                {
                    TempData["success"] = "Sign in successful.";
                    return RedirectToAction("Index", "Home", new { area = "Site" });
                }
                else
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid role.", StatusCode = 403 });
                }
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}", StatusCode = 500 });
            }
        }
        [HttpGet("Signin")]
        [AllowAnonymous]
        public IActionResult Signin() => View();

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(RegisterVM registerDto)
        {
            try
            {
                if (registerDto == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data.", StatusCode = 400 });

                var result = await _authService.RegisterUser(registerDto);
                if (!result.Success)
                {
                    TempData["error"] = result.Message;
                    return View("Register");
                }

                TempData["success"] = "Register successful.";
                return RedirectToAction("Signin", "Account", new { area = "Site" });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("Register")]
        [AllowAnonymous]
        public IActionResult Register() => View();

        [HttpGet("SignOut")]
        [AllowAnonymous]
        public async Task<IActionResult> SignOut()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    await _authService.LogoutUser(Guid.Parse(userId));
                }

                // Xóa Auth cookies
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                // Xóa cookies chứa token
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
                Response.Cookies.Delete("userId");

                TempData["success"] = "Sign out successful.";

                // Chuyển hướng về trang login
                return RedirectToAction("Signin", "Account", new { area = "Site" });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}", StatusCode = 500 });
            }
        }
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token.", StatusCode = 400 });
            }

            var response = await _authService.VerifyEmail(token);

            if (!response.Success)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = response.Message, StatusCode = response.StatusCode });
            }
            return View("EmailVerified", response.Data);
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

        [HttpGet("Site/Account/google-callback")]
        public async Task<IActionResult> GoogleCallback(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });

                var result = await _authService.LoginWithGoogle(code);

                if (result == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Login failed." });

                #region Đưa vào claims
                // Giải mã Access Token để lấy role
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(result.AccessToken);

                var roleClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "Role")?.Value;

                // Tạo claims để đăng nhập cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.Data.UserId.ToString()),
                    new Claim(ClaimTypes.Name, result.Data.FullName),
                    new Claim(ClaimTypes.Email, result.Data.Email),
                    new Claim(ClaimTypes.Role, roleClaim ?? "Customer")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Đăng nhập để lưu claims vào session/cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                #endregion

                TempData["success"] = "Sign in successful.";
                return View("Index", "Home");
                //return RedirectToAction("Index", "Home", new { id = result.Data?.UserId, accessToken = result.AccessToken, refreshToken = result.RefreshToken });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }

        //Dang nhap bang OAuth2.0 Google
        [HttpGet("google-redirect")]
        public async Task<IActionResult> RedirectToGoogle()
        {
            try
            {
                var result = await _authService.RedirectGoogleConsentScreenAsync();
                Console.WriteLine($"Redirect URL: {result.Data}");
                if (result.Success)
                {

                    return Redirect(result.Data);
                }
                else
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = result.Message });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
        [HttpGet("google-callback")]
        public async Task<IActionResult> HandleGoogleCallback(string code)
        {
            try
            {
                var result = await _authService.LoginWithGoogle(code);
                if (!result.Success)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = result.Message, StatusCode = result.StatusCode });
                }

                // Giữ JWT token trong cookie để sử dụng cho API calls
                Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(1)
                });

                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                // Ghi thêm userId (không bắt buộc)
                Response.Cookies.Append("userId", result.Data.UserId.ToString(), new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                // Giải mã JWT để lấy thông tin người dùng
                var tokenHandler = _jwtUtils.ValidateToken(result.AccessToken);
                if (tokenHandler == null)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token.", StatusCode = 401 });
                }

                var claimsIdentity = tokenHandler.Identity as ClaimsIdentity;
                if (claimsIdentity == null)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token.", StatusCode = 401 });
                }

                // Tạo identity mới từ claims trong token
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                };

                // Đăng nhập người dùng vào ASP.NET Identity
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Kiểm tra vai trò
                var roleClaim = claimsIdentity.FindFirst(ClaimTypes.Role);
                if (roleClaim == null)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Role claim not found.", StatusCode = 401 });
                }

                if (roleClaim.Value == "Admin")
                {
                    TempData["success"] = "Sign in successful.";
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else if (roleClaim.Value == "User")
                {
                    TempData["success"] = "Sign in successful.";
                    return RedirectToAction("Index", "Home", new { area = "Site" });
                }
                else
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid role.", StatusCode = 403 });
                }
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword() => View();
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(UserVM.ForgotPasswordVM forgotPasswordVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data.", StatusCode = 400 });
                }
                var result = await _authService.ForgotPassword(forgotPasswordVM);
                if (result.Success)
                {
                    return RedirectToAction("Signin", "Account", new { area = "Site" });
                }
                else
                {
                    return View("Error", new ErrorViewModel { StatusCode = result.StatusCode, ErrorMessage = result.Message });
                }
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}", StatusCode = 500 });
            }
        }
        [HttpGet("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token.", StatusCode = 400 });
            }
            return View(new UserVM.ResetPasswordVM { Token = token });
        }
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(UserVM.ResetPasswordVM resetPasswordVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data.", StatusCode = 400 });
                }
                var result = await _authService.ResetPassword(resetPasswordVM);
                if (result.Success)
                {
                    ViewData["Success"] = "Password reset successfully. You can now log in with your new password.";
                    return RedirectToAction("Signin", "Account", new { area = "Site" });
                }
                else
                {
                    ViewData["Error"] = result.Message;
                    return View("Error", new ErrorViewModel { ErrorMessage = result.Message, StatusCode = result.StatusCode });
                }
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}", StatusCode = 500 });
            }
        }
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
