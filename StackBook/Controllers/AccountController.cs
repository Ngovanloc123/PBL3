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
    [Route("[controller]")]

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
                Console.WriteLine($"SignIn result: {signInDto}");
                var result = await _authService.SignInUser(signInDto);
                Console.WriteLine($"SignIn result: {result.Data}");
                if (result.Success == false)
                    return View("Error", new ErrorViewModel { ErrorMessage = "Login failed." });
                //tra ve accessToken = result.AccessToken, refreshToken = result.RefreshToken });
                ViewData["AccessToken"] = result.AccessToken;
                ViewData["RefreshToken"] = result.RefreshToken;
                return RedirectToAction("Profile", new { id = result.Data?.UserId});
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
        [HttpPut("Update/{userId}")]
        [CustomAuthorize]
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
                return RedirectToAction("Profile", new { id = result.Data?.UserId});
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
        [HttpPut("UpdatePassword/{userId}")]
        [CustomAuthorize]
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
                var accessToken = result.AccessToken;
                ViewData["AccessToken"] = accessToken;
                return RedirectToAction("Profile", new { id = result.Data?.UserId});
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = $"Internal error: {ex.Message}" });
            }
        }
        [HttpGet("Profile/{id}")]
        [CustomAuthorize]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing.");
            }

            // Giải mã và xác thực token
            var claimsPrincipal = _jwtUtils.ValidateToken(token);
            if (claimsPrincipal == null)
            {
                return Unauthorized("Invalid token.");
            }

            // Tiếp tục xử lý sau khi xác thực token
            var userProfile = await _userService.GetUserById(id);
            if (userProfile == null)
            {
                return NotFound();
            }

            return View(userProfile);
        }
        [HttpGet("Logout")]
        [CustomAuthorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Token is missing.");
                }

                var claimsPrincipal = _jwtUtils.ValidateToken(token);
                if (claimsPrincipal == null)
                {
                    return Unauthorized("Invalid token.");
                }

                var userId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
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
