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
        public async Task<IActionResult> RegisterUser(RegisterDto registerDto)
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

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInUser(SignInDto signInDto)
        {
            try
            {
                if (signInDto == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });

                var result = await _authService.SignInUser(signInDto);
                if (result.Success == false)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Login failed." });

                TempData["AccessToken"] = result.AccessToken;
                System.Console.WriteLine(result.AccessToken);
                TempData["RefreshToken"] = result.RefreshToken;
                System.Console.WriteLine(result.RefreshToken);
                TempData["UserId"] = result.Data?.UserId;
                System.Console.WriteLine(result.Data?.UserId);
                return RedirectToAction("Profile", new { id = result.Data?.UserId });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }

        [HttpPut("Update/{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid userId, UpdateDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });

                var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (currentUserIdClaims == null)
                    return Unauthorized("User not authenticated.");

                var currentUserId = Guid.Parse(currentUserIdClaims);
                if (userId != currentUserId)
                    return View("Error", new ErrorViewModel { ErrorMessage = "You can only update your own profile." });

                updateDto.UserId = userId;
                var result = await _userService.UpdateUser(updateDto);
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
        public async Task<IActionResult> UpdatePassword(Guid userId, UpdatePasswordDto updatePasswordDto)
        {
            try
            {
                if (updatePasswordDto == null)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Invalid data." });

                var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (currentUserIdClaims == null)
                    return Unauthorized("User not authenticated.");

                var currentUserId = Guid.Parse(currentUserIdClaims);
                if (userId != currentUserId)
                    return View("Error", new ErrorViewModel { ErrorMessage = "You can only update your own password." });

                updatePasswordDto.UserId = userId;
                var result = await _userService.UpdatePassword(updatePasswordDto);
                ViewData["AccessToken"] = result.AccessToken;
                return RedirectToAction("Profile", new { id = result.Data?.UserId });
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }

        [HttpGet("Profile/{id}")]
        [Authorize]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            // Lấy thông tin người dùng từ context.User đã được xác thực trong middleware
            var userProfile = await _userService.GetUserById(id);
            if (userProfile == null)
            {
                return NotFound();
            }

            return View(userProfile);
        }
        [HttpGet("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }

                await _authService.LogoutUser(Guid.Parse(userId));
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
