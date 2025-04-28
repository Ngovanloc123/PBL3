using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using StackBook.Utils;

namespace StackBook.Middleware
{
    public class Authentication
    {
        private readonly RequestDelegate _next;

        public Authentication(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, JwtUtils jwtUtils)
        {
            // Lấy thông tin Authorization header
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            
            // Nếu có header Authorization và bắt đầu bằng "Bearer"
            if (authHeader != null && IsBearerToken(authHeader))
            {
                var token = ExtractToken(authHeader);

                // Xác thực và lấy principal từ JWT token
                var principal = jwtUtils.ValidateToken(token);

                // Nếu principal hợp lệ, gán vào context.User
                if (principal != null)
                {
                    context.User = principal;
                    context.Items["User"] = principal;
                }
                else
                {
                    // Nếu không xác thực được token, có thể thêm xử lý lỗi hoặc trả về 401.
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
            }

            // Tiếp tục chuỗi middleware
            await _next(context);
        }

        // Phương thức kiểm tra xem header có phải là Bearer token không
        private bool IsBearerToken(string authHeader)
        {
            return !string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);
        }

        // Phương thức tách token từ Authorization header
        private string ExtractToken(string authHeader)
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
    }
}
