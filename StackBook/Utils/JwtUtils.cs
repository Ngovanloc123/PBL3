using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackBook.Models;

namespace StackBook.Utils
{
    public class JwtUtils
    {
        private readonly string _secretKey;
        private readonly int _tokenExpiryDays;

        public JwtUtils(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Secret key is not configured.");
            _tokenExpiryDays = int.TryParse(configuration["Jwt:TokenExpiryDays"], out var days) ? days : 1;
        }

        //Kiểm tra token có dùng thuật toán hợp lệ không
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validateVMken)
        {
            return validateVMken is JwtSecurityToken jwt &&
                   (jwt.Header.Alg == SecurityAlgorithms.HmacSha256 || jwt.Header.Alg == SecurityAlgorithms.HmacSha512);
        }

        //Xác thực JWT Token
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParams, out var validateVMken);
                return IsJwtWithValidSecurityAlgorithm(validateVMken) ? principal : null;
            }
            catch (SecurityTokenExpiredException) { Console.WriteLine("Token đã hết hạn."); }
            catch (SecurityTokenInvalidSignatureException) { Console.WriteLine("Token có chữ ký không hợp lệ."); }
            catch (Exception ex) { Console.WriteLine($"Xác thực token thất bại: {ex.Message}"); }

            return null;
        }

        //Sinh danh sách Claims cho user
        protected virtual List<Claim> GenerateClaimsForUser(User user) => new()
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role ? "Admin" : "Customer"),
            new Claim("IsEmailVerified", user.IsEmailVerified ? "active" : "no-active"),
            new Claim("LockStatus", user.LockStatus ? "lock" : "no-lock"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        //Dựng thông tin token
        protected virtual SecurityTokenDescriptor BuilVMkenDescriptor(List<Claim> claims, DateTime expiresAt)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                SigningCredentials = credentials
            };
        }

        //Sinh   (30 phút)
        public string GenerateAccessToken(User user)
        {
            var claims = GenerateClaimsForUser(user);
            var tokenDescriptor = BuilVMkenDescriptor(claims, DateTime.UtcNow.AddMinutes(30));
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //Sinh refresh token (ngẫu nhiên, không chứa user info)
        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        //Sinh token dùng để reset mật khẩu (10 phút)
        public string GenerateResetToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("ResetPassword", "true")
            };

            var tokenDescriptor = BuilVMkenDescriptor(claims, DateTime.UtcNow.AddMinutes(10));
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //Sinh token mặc định theo cấu hình ngày hết hạn
        public string GenerateToken(User user)
        {
            var claims = GenerateClaimsForUser(user);
            var tokenDescriptor = BuilVMkenDescriptor(claims, DateTime.UtcNow.AddDays(_tokenExpiryDays));
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
