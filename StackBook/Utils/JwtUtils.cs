using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.IdentityModel.Tokens;
using StackBook.Models;
using Microsoft.Extensions.Configuration;

namespace StackBook.Utils
{
    public class JwtUtils
    {
        private readonly string _secretKey;
        private readonly int _tokenExpiryDays;

        public JwtUtils(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Secret key is not configured.");
            _tokenExpiryDays = int.TryParse(configuration["Jwt:TokenExpiryDays"], out var expiryDays) ? expiryDays : 1;
        }
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            if (validatedToken is JwtSecurityToken jwtToken)
            {
                var algorithm = jwtToken.Header.Alg;
                return algorithm == SecurityAlgorithms.HmacSha256 || algorithm == SecurityAlgorithms.HmacSha512;
            }

            return false;
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.UTF8.GetBytes(_secretKey); 

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,     // Nếu có issuer hợp lệ, thay đổi thành true và cấu hình issuer
                ValidateAudience = false,   // Nếu có audience hợp lệ, thay đổi thành true và cấu hình audience
                ValidateLifetime = true,    // Kiểm tra thời gian hết hạn của token
                ClockSkew = TimeSpan.Zero   // Không chấp nhận độ lệch thời gian
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return principal;
                }
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("Token đã hết hạn.");
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                Console.WriteLine("Chữ ký của token không hợp lệ.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xác thực token thất bại: {ex.Message}");
            }

            return null;
        }

        //Generate JWT Token
        protected virtual List<Claim> GenerateClaimsForUser(User user)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Role", user.Role == true ? "admin": "user"),
                new Claim("IsEmailVerified", user.IsEmailVerified == false ? "no-active" : "active"),
                new Claim("LockStatus", user.LockStatus == false ? "no-lock" : "lock"),
            };
        }

        protected virtual SecurityTokenDescriptor BuildTokenDescriptor(User user, List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(_tokenExpiryDays),
                SigningCredentials = credentials
            };
            return tokenDescriptor;
        }

        public virtual string GenerateToken(User user)
        {
            var claims = GenerateClaimsForUser(user);
            var identity = new ClaimsIdentity(claims);
            var tokenDescriptor = BuildTokenDescriptor(user, claims);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
