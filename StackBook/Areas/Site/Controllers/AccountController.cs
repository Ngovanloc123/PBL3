using Microsoft.AspNetCore.Mvc;
using StackBook.VMs;
using StackBook.Services;
using StackBook.Models;
using System.Threading.Tasks;
using StackBook.Middleware;
using Microsoft.AspNetCore.Authorization;
using StackBook.ViewModels;
using StackBook.Interfaces;
using StackBook.Utils;
using static StackBook.ViewModels.UserVM;
using Azure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace StackBook.Areas.Site.Controllers
{
    [Area("Site")]
    //[Route("Site/Account ")]
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
        [HttpGet("Signin")]
        public IActionResult Signin(string? urlRedirect)
        {
            ViewBag.URLRedirect = urlRedirect;
            return View();
        }
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", registerVM);
            }
            var result = await _authService.RegisterUser(registerVM);
            if (result.Success == false)
            {
                TempData["error"] = result.Message;
                return View("Register", registerVM);
            }
            return RedirectToAction("Signin");
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInUser(SignInVM signInVM, string? urlRedirect)
        {
            ViewBag.URLRedirect = urlRedirect;
            if (!ModelState.IsValid)
            {
                return View("Signin", signInVM);
            }

            var result = await _authService.SignInUser(signInVM);
            if (!result.Success)
            {
                TempData["error"] = result.Message;
                return View("Signin", signInVM);
            }

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

            // Ghi thêm userId (không bắt buộc)
            Response.Cookies.Append("userId", result.Data.UserId.ToString(), new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });


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


            // Chuyển hướng theo role hoặc urlRedirect
            if (!string.IsNullOrEmpty(urlRedirect) && Url.IsLocalUrl(urlRedirect))
            {
                return Redirect(urlRedirect);
            }

            TempData["success"] = "Sign in successful.";
            if (roleClaim == "Admin")
            {
                return RedirectToAction("Index", "Statistic", new { area = "Admin" });
            }
            else // Customer hoặc role khác
            {
                return RedirectToAction("Index", "Home", new { area = "Site" });
            }
        }



        [HttpPut("Update/{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid userId, UpdateVM updateVM)
        {
            try
            {
                if (updateVM == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });

                var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (currentUserIdClaims == null)
                    return Unauthorized("User not authenticated.");

                var currentUserId = Guid.Parse(currentUserIdClaims);
                if (userId != currentUserId)
                    return View("Error", new ErrorViewModel { ErrorMessage = "You can only update your own profile." });

                updateVM.UserId = userId;
                var result = await _userService.UpdateUser(updateVM);
                ViewData["AccessToken"] = result.AccessToken;
                return RedirectToAction("Profile", new { id = result.Data?.UserId });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }

        [HttpPut("UpdatePassword/{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword(Guid userId, UpdatePasswordVM updatePasswordVM)
        {
            try
            {
                if (updatePasswordVM == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });

                var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (currentUserIdClaims == null)
                    return Unauthorized("User not authenticated.");

                var currentUserId = Guid.Parse(currentUserIdClaims);
                if (userId != currentUserId)
                    return View("Error", new ErrorViewModel { ErrorMessage = "You can only update your own password." });

                updatePasswordVM.UserId = userId;
                var result = await _userService.UpdatePassword(updatePasswordVM);
                ViewData["AccessToken"] = result.AccessToken;
                return RedirectToAction("Profile", new { id = result.Data?.UserId });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }


        //[HttpGet("Logout")]
        //[Authorize]
        //public async Task<IActionResult> Signout()
        //{
        //    try
        //    {
        //        var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        //        if (userId == null)
        //        {
        //            return Unauthorized("User not authenticated.");
        //        }

        //        await _authService.LogoutUser(Guid.Parse(userId));
        //        return RedirectToAction("Login");
        //    }
        //    catch (Exception ex)
        //    {
        //        return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
        //    }
        //}

        [HttpGet("Signout")]
        [Authorize]
        public async Task<IActionResult> Signout()
        {
            try
            {
                // Xóa tất cả các cookie liên quan đến xác thực
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
                Response.Cookies.Delete("userId");

                // Xóa thông tin xác thực khỏi HttpContext.User
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Chuyển hướng đến trang đăng nhập
                TempData["success"] = "You have been signed out successfully.";
                return RedirectToAction("Signin");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và hiển thị thông báo lỗi
                return View("Error", new ErrorViewModel { ErrorMessage = $"An error occurred during signout: {ex.Message}" });
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
                return RedirectToAction("Index", "Home", new { id = result.Data?.UserId, accessToken = result.AccessToken, refreshToken = result.RefreshToken });
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
                   

                    return RedirectToAction("Profile", "Account", new { area = "Customer", id = result.Data?.UserId });
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
    }
}


//using Microsoft.AspNetCore.Mvc;
//using StackBook.VMs;
//using StackBook.Services;
//using StackBook.Models;
//using System.Threading.Tasks;
//using StackBook.Middleware;
//using Microsoft.AspNetCore.Authorization;
//using StackBook.ViewModels;
//using StackBook.Interfaces;
//using StackBook.Utils;
//using static StackBook.ViewModels.UserVM;
//using Azure;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication;

//namespace StackBook.Areas.Site.Controllers
//{
//    [Area("Site")]
//    [Route("Auth/[controller]")]
//    public class AccountController : Controller
//    {
//        private readonly IUserService _userService;
//        private readonly IAuthService _authService;
//        private readonly JwtUtils _jwtUtils;
//        private readonly IHttpContextAccessor _httpContextAccessor;

//        public AccountController(IUserService userService, IAuthService authService, IHttpContextAccessor httpContextAccessor, JwtUtils jwtUtils)
//        {
//            _userService = userService;
//            _authService = authService;
//            _httpContextAccessor = httpContextAccessor;
//            _jwtUtils = jwtUtils;
//        }

//        public IActionResult Signin(string? urlRedirect)
//        {
//            ViewBag.URLRedirect = urlRedirect;
//            return View();
//        }

//        public IActionResult Register()
//        {
//            return View();
//        }

//        [HttpPost("Register")]
//        public async Task<IActionResult> RegisterUser(RegisterVM registerVM)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View("Register", registerVM);
//            }
//            var result = await _authService.RegisterUser(registerVM);
//            if (result.Success == false)
//            {
//                TempData["error"] = result.Message;
//                return View("Register", registerVM);
//            }
//            return RedirectToAction("Signin");
//        }

//        [HttpPost("SignIn")]
//        public async Task<IActionResult> SignInUser(SignInVM signInVM, string? urlRedirect)
//        {
//            ViewBag.URLRedirect = urlRedirect;
//            if (!ModelState.IsValid)
//            {
//                return View("Signin", signInVM);
//            }

//            var result = await _authService.SignInUser(signInVM);
//            if (!result.Success)
//            {
//                TempData["error"] = result.Message;
//                return View("Signin", signInVM);
//            }

//            // Ghi access token vào cookie
//            Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
//            {
//                HttpOnly = true,
//                Secure = true,
//                SameSite = SameSiteMode.Strict,
//                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
//            });

//            // Ghi refresh token vào cookie
//            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
//            {
//                HttpOnly = true,
//                Secure = true,
//                SameSite = SameSiteMode.Strict,
//                Expires = DateTimeOffset.UtcNow.AddDays(7)
//            });

//            // Ghi thêm userId (không bắt buộc)
//            Response.Cookies.Append("userId", result.Data.UserId.ToString(), new CookieOptions
//            {
//                HttpOnly = false,
//                Secure = true,
//                SameSite = SameSiteMode.Strict,
//                Expires = DateTimeOffset.UtcNow.AddDays(7)
//            });

//            // Giải mã Access Token để lấy role
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var jwtToken = tokenHandler.ReadJwtToken(result.AccessToken);

//            var roleClaim = jwtToken.Claims.FirstOrDefault(c =>
//                c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "Role")?.Value;

//            // Tạo claims để đăng nhập cookie
//            var claims = new List<Claim>
//                    {
//                        new Claim(ClaimTypes.NameIdentifier, result.Data.UserId.ToString()),
//                        new Claim(ClaimTypes.Name, result.Data.FullName),
//                        new Claim(ClaimTypes.Email, result.Data.Email),
//                        new Claim(ClaimTypes.Role, roleClaim ?? "Customer")
//                    };

//            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//            var principal = new ClaimsPrincipal(identity);

//            // Đăng nhập để lưu claims vào session/cookie
//            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

//            // Chuyển hướng theo role hoặc urlRedirect
//            if (!string.IsNullOrEmpty(urlRedirect) && Url.IsLocalUrl(urlRedirect))
//            {
//                return Redirect(urlRedirect);
//            }

//            TempData["success"] = "Sign in successful.";
//            if (roleClaim == "Admin")
//            {
//                return RedirectToAction("Index", "Statistic", new { area = "Admin" });
//            }
//            else // Customer hoặc role khác
//            {
//                return RedirectToAction("Index", "Home", new { area = "Site" });
//            }
//        }
//        [HttpGet("Signout")]
//        [Authorize]
//        public async Task<IActionResult> Signout()
//        {
//            try
//            {
//                // Xóa tất cả các cookie liên quan đến xác thực
//                Response.Cookies.Delete("accessToken");
//                Response.Cookies.Delete("refreshToken");
//                Response.Cookies.Delete("userId");

//                // Xóa thông tin xác thực khỏi HttpContext.User
//                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

//                // Chuyển hướng đến trang đăng nhập
//                TempData["success"] = "You have been signed out successfully.";
//                return RedirectToAction("Signin");
//            }
//            catch (Exception ex)
//            {
//                // Xử lý lỗi và hiển thị thông báo lỗi
//                return View("Error", new ErrorViewModel { ErrorMessage = $"An error occurred during signout: {ex.Message}" });
//            }
//        }
//        [HttpGet("verify-email")]
//        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
//        {
//            if (string.IsNullOrEmpty(token))
//            {
//                return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token." });
//            }

//            var response = await _authService.VerifyEmail(token);

//            if (!response.Success)
//            {
//                return View("Error", new ErrorViewModel { ErrorMessage = response.Message });
//            }
//            return View("EmailVerified", response.Data);
//        }
//        //Dang nhap bang OAuth2.0 Google
//        [HttpGet("google-redirect")]
//        public async Task<IActionResult> RedirectToGoogle()
//        {
//            try
//            {
//                var result = await _authService.RedirectGoogleConsentScreenAsync();
//                Console.WriteLine($"Redirect URL: {result.Data}");
//                if (result.Success)
//                {
//                    return Redirect(result.Data);
//                }
//                else
//                {
//                    return View("Error", new ErrorViewModel { ErrorMessage = result.Message });
//                }
//            }
//            catch (Exception ex)
//            {
//                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
//            }
//        }
//        [HttpGet("google-callback")]
//        public async Task<IActionResult> HandleGoogleCallback(string code)
//        {
//            try
//            {
//                var result = await _authService.LoginWithGoogle(code);
//                if (result.Success)
//                {
//                    // Ghi access token vào cookie
//                    Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
//                    {
//                        HttpOnly = true,
//                        Secure = true,
//                        SameSite = SameSiteMode.Strict,
//                        Expires = DateTimeOffset.UtcNow.AddDays(1)
//                    });
//                    // Ghi refresh token vào cookie
//                    Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
//                    {
//                        HttpOnly = true,
//                        Secure = true,
//                        SameSite = SameSiteMode.Strict,
//                        Expires = DateTimeOffset.UtcNow.AddDays(7)
//                    });
//                    // Nếu muốn: Ghi thêm user ID (không bắt buộc)
//                    Response.Cookies.Append("userId", result.Data.UserId.ToString(), new CookieOptions
//                    {
//                        HttpOnly = false,
//                        Secure = true,
//                        SameSite = SameSiteMode.Strict,
//                        Expires = DateTimeOffset.UtcNow.AddDays(7)
//                    });
//                    return RedirectToAction("Profile", "Account", new { area = "Customer", id = result.Data?.UserId });
//                    // return RedirectToAction("Index", "Home", new { area = "Site" });
//                }
//                else
//                {
//                    return View("Error", new ErrorViewModel { ErrorMessage = result.Message });
//                }
//            }
//            catch (Exception ex)
//            {
//                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
//            }
//        }
//        [HttpGet("forgot-password")]
//        [AllowAnonymous]
//        public IActionResult ForgotPassword() => View();
//        [HttpPost("forgot-password")]
//        [AllowAnonymous]
//        public async Task<IActionResult> ForgotPassword(UserVM.ForgotPasswordVM forgotPasswordVM)
//        {
//            try
//            {
//                if (!ModelState.IsValid)
//                {
//                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });
//                }
//                var result = await _authService.ForgotPassword(forgotPasswordVM);
//                if (result.Success)
//                {
//                    return RedirectToAction("Signin", "Account", new { area = "Site" });
//                }
//                else
//                {
//                    return View("Error", new ErrorViewModel { ErrorMessage = result.Message });
//                }
//            }
//            catch (Exception ex)
//            {
//                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
//            }
//        }
//        [HttpGet("reset-password")]
//        [AllowAnonymous]
//        public IActionResult ResetPassword(string token)
//        {
//            if (string.IsNullOrEmpty(token))
//            {
//                return View("Error", new ErrorViewModel { ErrorMessage = "Invalid token." });
//            }
//            return View(new UserVM.ResetPasswordVM { Token = token });
//        }
//        [HttpPost("reset-password")]
//        [AllowAnonymous]
//        public async Task<IActionResult> ResetPassword(UserVM.ResetPasswordVM resetPasswordVM)
//        {
//            try
//            {
//                if (!ModelState.IsValid)
//                {
//                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });
//                }
//                var result = await _authService.ResetPassword(resetPasswordVM);
//                if (result.Success)
//                {
//                    ViewData["Success"] = "Password reset successfully. You can now log in with your new password.";
//                    return RedirectToAction("Signin", "Account", new { area = "Site" });
//                }
//                else
//                {
//                    ViewData["Error"] = result.Message;
//                    return View("Error", new ErrorViewModel { ErrorMessage = result.Message });
//                }
//            }
//            catch (Exception ex)
//            {
//                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
//            }
//        }
//    }
//}