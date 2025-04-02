using Microsoft.AspNetCore.Mvc;
using StackBook.DTOs;
using StackBook.Services;
using System.Threading.Tasks;
using StackBook.Middleware;
using Microsoft.AspNetCore.Authorization;
namespace StackBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        // Tiêm UserService qua constructor
        public UserController(UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        // Phương thức đăng ký người dùng
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                return BadRequest("Invalid data.");
            }

            var result = await _userService.RegisterUser(registerDto);

            return Ok(result);  // Trả về thông báo thành công khi đăng ký
        }
        // Phương thức đăng nhập người dùng
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest("Invalid data.");
            }

            var result = await _userService.LoginUser(loginDto);

            return Ok(result);  // Trả về thông báo thanh cong khi đăng nhập
        }
        // Phương thức chỉnh sửa thống tin người dùng
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId,[FromBody] UpdateDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest("Invalid data.");
            }

            var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            
            if(currentUserIdClaims == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var currentUserId = Guid.Parse(currentUserIdClaims);

            if (userId != currentUserId)
            {
                return BadRequest("You can only update your own profile.");
            }

            updateDto.UserId = userId;
            
            var result = await _userService.UpdateUser(updateDto);
            
            return Ok(result);
        }
        //Phuong thuc cap nhat mat khau nguoi dung
        [HttpPut("password/{userId}")] 
        public async Task<IActionResult> UpdatePassword(Guid userId, [FromBody] UpdatePasswordDto updatePasswordDto)
        {
            if (updatePasswordDto == null)
            {
                return BadRequest("Invalid data.");
            }
            var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (currentUserIdClaims == null)
            {
                return Unauthorized("User not authenticated.");
            }
            var currentUserId = Guid.Parse(currentUserIdClaims);
            if (userId != currentUserId)
            {
                return BadRequest("You can only update your own password.");
            }
            updatePasswordDto.UserId = userId;
            var result = await _userService.UpdatePassword(updatePasswordDto);
            return Ok(result);
        }
        //Phuong thuc xoa nguoi dung
        [HttpDelete("{userId}")] 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var currentUserIdClaims = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (currentUserIdClaims == null)
            {
                return Unauthorized("User not authenticated.");
            }
            var currentUserId = Guid.Parse(currentUserIdClaims);
            if (userId != currentUserId)
            {
                return BadRequest("You can only delete your own account.");
            }
            var result = await _userService.DeleteUser(userId);
            return Ok(result);
        }
        [HttpDelete("self")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> DeleteSelf()
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? string.Empty);
            var result = await _userService.DeleteUser(userId);
            if(result == null)
            {
                return Unauthorized("User not authenticated.");
            }
            return Ok("Deleted successfully.");
        }
    }
}
