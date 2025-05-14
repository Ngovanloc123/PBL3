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
                    Expires = DateTimeOffset.UtcNow.AddMinutes(60)
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
                //Check Role roi chuyen den area tuong ung
                //validate access token de lay role
                var tokenHandler = _jwtUtils.ValidateToken(result.AccessToken);
                if (tokenHandler == null)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token." });
                }

                var claimsIdentity = tokenHandler.Identity as ClaimsIdentity;

                if (claimsIdentity == null)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token." });
                }
                var roleClaim = claimsIdentity.FindFirst(ClaimTypes.Role);
                if (roleClaim == null)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Role claim not found." });
                }
                if(roleClaim.Value == "Admin")
                {
                    TempData["success"] = "Sign in successful.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else if (roleClaim.Value == "User")
                {
                    TempData["success"] = "Sign in successful.";
                    // return RedirectToAction("Index", "Home", new { area = "Site" });
                    return RedirectToAction("Profile", "Account", new { area = "Customer"});
                }
                else
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid role." });
                }
                // return RedirectToAction("Profile", "Account", new { area = "Customer", id = result.Data?.UserId });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
        [HttpGet("Signin")]
        [AllowAnonymous]
        public IActionResult Signin() => View();

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
                
                return RedirectToAction("Signin", "Account", new { area = "Site" });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }

        [HttpGet("Register")]
        [AllowAnonymous]
        public IActionResult Register() => View();

        [HttpGet("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            try
            {
                // Lấy userId từ claim trong token (cookie)
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }

                // Đăng xuất người dùng (thực hiện các hành động cần thiết từ phía server, ví dụ: hủy session, xóa refresh token...)
                await _authService.LogoutUser(Guid.Parse(userId));

                // Xóa cookie chứa access token khi người dùng đăng xuất
                Response.Cookies.Delete("accessToken");  // Xóa accessToken khỏi cookie
                Response.Cookies.Delete("refreshToken"); // Xóa refreshToken khỏi cookie (nếu có)
                Response.Cookies.Delete("userId");       // Xóa userId khỏi cookie

                // Chuyển hướng về trang login
                return RedirectToAction("Signin", "Account", new { area = "Site" });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token." });
            }

            var response = await _authService.VerifyEmail(token);

            if (!response.Success)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = response.Message });
            }
            return View("EmailVerified", response.Data);
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
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
        [HttpGet("google-callback")]
        public async Task<IActionResult> HandleGoogleCallback(string code)
        {
            try
            {
                var result = await _authService.LoginWithGoogle(code);
                if (result.Success)
                {
                    // Ghi access token vào cookie
                    Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(1)
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
                    return RedirectToAction("Profile", "Account", new { area = "Customer"});
                    // return RedirectToAction("Index", "Home", new { area = "Site" });
                }
                else
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = result.Message });
                }
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
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
               if(!ModelState.IsValid)
               {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });
               } 
               var result = await _authService.ForgotPassword(forgotPasswordVM);
               if (result.Success)
               {
                    return RedirectToAction("Signin", "Account", new { area = "Site" });
               }
               else
               {
                    return View("Error", new ErrorViewModel { ErrorMessage = result.Message });
               }
            } 
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
        [HttpGet("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token." });
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
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });
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
                    return View("Error", new ErrorViewModel { ErrorMessage = result.Message });
                }
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
    }
}
