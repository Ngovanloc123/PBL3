using Microsoft.AspNetCore.Mvc;
using StackBook.DTOs;
using StackBook.Services;
using System.Threading.Tasks;
using StackBook.Middleware;
using Microsoft.AspNetCore.Authorization;

namespace StackBook.Controllers
{
    [Route("auth/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (registerDto == null)
                    return BadRequest("Invalid data.");

                var result = await _userService.RegisterUser(registerDto);
                if (result == null)
                    return BadRequest("Registration failed.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                    return BadRequest("Invalid data.");

                var result = await _userService.LoginUser(loginDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return BadRequest("Invalid data.");

                var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (currentUserIdClaims == null)
                    return Unauthorized("User not authenticated.");

                var currentUserId = Guid.Parse(currentUserIdClaims);

                if (userId != currentUserId)
                    return BadRequest("You can only update your own profile.");

                updateDto.UserId = userId;
                var result = await _userService.UpdateUser(updateDto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpPut("password/{userId}")]
        public async Task<IActionResult> UpdatePassword(Guid userId, [FromBody] UpdatePasswordDto updatePasswordDto)
        {
            try
            {
                if (updatePasswordDto == null)
                    return BadRequest("Invalid data.");

                var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (currentUserIdClaims == null)
                    return Unauthorized("User not authenticated.");

                var currentUserId = Guid.Parse(currentUserIdClaims);
                if (userId != currentUserId)
                    return BadRequest("You can only update your own password.");

                updatePasswordDto.UserId = userId;
                var result = await _userService.UpdatePassword(updatePasswordDto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (currentUserIdClaims == null)
                    return Unauthorized("User not authenticated.");

                var currentUserId = Guid.Parse(currentUserIdClaims);
                if (userId != currentUserId)
                    return BadRequest("You can only delete your own account.");

                var result = await _userService.DeleteUser(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpDelete("self")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> DeleteSelf()
        {
            try
            {
                var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized("User not authenticated.");

                var userId = Guid.Parse(userIdStr);
                var result = await _userService.DeleteUser(userId);

                if (result == null)
                    return NotFound("User not found.");

                return Ok("Deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }
}
