using CourseManagementSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CourseManagementSystem.Application.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<object> _passwordHasher;

        public PasswordService()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(
            string password,
            string passwordHash)
        {
            var result =
                _passwordHasher.VerifyHashedPassword(
                    null!,
                    passwordHash,
                    password);

            return result == PasswordVerificationResult.Success;
        }
    }
}