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
        [HttpGet("Profile/{id}")]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> Profile(Guid id)
        {
            Console.WriteLine($"Profile ID: {id}");
            //User Id của người dùng hiện tại trong cookie
            var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"UserIdValue: {userIdValue}");
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });
            Guid userId = Guid.Parse(userIdValue);
            if (userId != id)
                return View("Error", new ErrorViewModel { ErrorMessage = "You do not have permission to view this profile." });
            var response = await _userService.GetUserById(id);
            if (response == null) return NotFound();
            return View(response.Data);
        }
        [HttpPost("UpdateAvatar/{id}")]
        public async Task<IActionResult> UpdateAvatar(Guid id, IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");
            var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
                return View("Error", new ErrorViewModel { ErrorMessage = "User ID not found." });
            Guid userId = Guid.Parse(userIdValue);
            if (userId != id)
                return View("Error", new ErrorViewModel { ErrorMessage = "You do not have permission to update this profile." });
            var response = await _userService.UpdateAvatar(id, image);
            if (response.Success)
            {
                return RedirectToAction("Profile", new { id });
            }
            else
            {
                return View("Error", new ErrorViewModel { ErrorMessage = response.Message });
            }
        }
    }
}
