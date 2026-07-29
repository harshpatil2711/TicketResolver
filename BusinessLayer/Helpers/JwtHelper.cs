using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TicketResolver.Helpers
{
    public static class JwtHelper
    {
        private static string JwtSecret => ConfigurationManager.AppSettings["JwtSecret"];
        private static int AccessTokenExpiryMinutes => int.Parse(ConfigurationManager.AppSettings["AccessTokenExpiryMinutes"] ?? "15");
        private static int RefreshTokenExpiryDays => int.Parse(ConfigurationManager.AppSettings["RefreshTokenExpiryDays"] ?? "7");

        public static string GenerateAccessToken(int userId, string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: ConfigurationManager.AppSettings["JwtIssuer"],
                audience: ConfigurationManager.AppSettings["JwtAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public static string HashRefreshToken(string refreshToken)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
                return Convert.ToBase64String(bytes);
            }
        }

        public static ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(JwtSecret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = ConfigurationManager.AppSettings["JwtIssuer"],
                ValidateAudience = true,
                ValidAudience = ConfigurationManager.AppSettings["JwtAudience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken securityToken;
            return tokenHandler.ValidateToken(token, validationParameters, out securityToken);
        }

        public static int GetRefreshTokenExpiryDays()
        {
            return RefreshTokenExpiryDays;
        }
    }
}
