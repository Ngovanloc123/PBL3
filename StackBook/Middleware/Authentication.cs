using Microsoft.AspNetCore.Http;
using StackBook.Utils;
using System.Threading.Tasks;

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
            // Lấy access token từ cookie
            var token = context.Request.Cookies["accessToken"];

            if (!string.IsNullOrEmpty(token))
            {
                // Xác thực token và lấy ClaimsPrincipal
                var principal = jwtUtils.ValidateToken(token);

                if (principal != null)
                {
                    context.User = principal;
                    context.Items["User"] = principal;
                }
                else
                {
                    // Token không hợp lệ => trả về 401
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            // Tiếp tục xử lý request
            await _next(context);
        }
    }
}
