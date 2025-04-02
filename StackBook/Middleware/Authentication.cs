using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using StackBook.Utils;

namespace  StackBook.Middleware
{
    public class Authentication
    {
        private readonly RequestDelegate _next;
        private readonly JwtUtils _jwtUtils;

        public Authentication(RequestDelegate next, JwtUtils jwtUtils)
        {
            _next = next;
            _jwtUtils = jwtUtils;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if(!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer".Length).Trim();
                // Validate the token
                var principal = _jwtUtils.ValidateToken(token);
                if(principal != null)
                {
                    context.Items["User"] = principal;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }
            else
            {
                // No token found
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
            await _next(context);
        }
    }    
}