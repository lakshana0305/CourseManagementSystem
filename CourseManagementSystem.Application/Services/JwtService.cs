using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CourseManagementSystem.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CourseManagementSystem.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(
            int userId,
            string email,
            string role)
        {
            var key =
                _configuration["Jwt:Key"]
                ?? throw new Exception("JWT key is missing.");

            var issuer =
                _configuration["Jwt:Issuer"];

            var audience =
                _configuration["Jwt:Audience"];

            var expiryMinutes =
                int.Parse(
                    _configuration["Jwt:ExpiryMinutes"] ?? "60");

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    email),

                new Claim(
                    ClaimTypes.Role,
                    role)
            };

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key));

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}