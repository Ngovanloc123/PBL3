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
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var principal = jwtUtils.ValidateToken(token);

                if (principal != null)
                {
                    context.User = principal;
                    context.Items["User"] = principal;
                }
            }

            await _next(context);
        }
    }
}
