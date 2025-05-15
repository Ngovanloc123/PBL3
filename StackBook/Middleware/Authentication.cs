using Microsoft.AspNetCore.Http;
using NuGet.Protocol;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;
using StackBook.Utils;
using System.Threading.Tasks;
using StackBook.Models;
using Azure;

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
                    //check Refresh token
                    var refreshToken = context.Request.Cookies["refreshToken"];
                    //Kiem tra refresh token co con hieu luc hay khong
                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        context.Response.Cookies.Delete("accessToken");
                        context.Response.Cookies.Delete("refreshToken");
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                    var userRepository = context.RequestServices.GetRequiredService<IUserRepository>();
                    var existingUser = await userRepository.GetUserByRefreshTokenAsync(refreshToken);
                    //in ra thong tin user
                    Console.WriteLine($"Refresh token: {existingUser?.RefreshToken}");
                    if (existingUser == null)
                    {
                        context.Response.Cookies.Delete("accessToken");
                        context.Response.Cookies.Delete("refreshToken");
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                    //Kiem tra ngay het han refresh token
                    var dateNow = DateTime.UtcNow;
                    var dateRefreshToken = existingUser.RefreshTokenExpiry;
                    if (dateNow > dateRefreshToken)
                    {
                        context.Response.Cookies.Delete("accessToken");
                        context.Response.Cookies.Delete("refreshToken");
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                    // Tạo token mới
                    var newAccessToken = jwtUtils.GenerateAccessToken(existingUser);
                    context.Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTime.UtcNow.AddMinutes(15)
                    });

                    // Validate lại token mới và set lại context.User
                    var newPrincipal = jwtUtils.ValidateToken(newAccessToken);
                    if (newPrincipal != null)
                    {
                        context.User = newPrincipal;
                        context.Items["User"] = newPrincipal;
                    }

                    // Tiếp tục xử lý request
                    await _next(context);
                    return;
                }
            }
            // Tiếp tục xử lý request
            await _next(context);
        }
    }
}
