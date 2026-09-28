using CourseManagementSystem.Application.DTOs.User;
using CourseManagementSystem.Application.Services.Interfaces;
using CourseManagementSystem.Infrastructure.Repositories.Interfaces;

namespace CourseManagementSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> LoginAsync(
            LoginUserDto dto)
        {
            var user =
                await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
            {
                throw new Exception(
                    "Invalid email or password.");
            }

            var isPasswordValid =
                _passwordService.VerifyPassword(
                    dto.Password,
                    user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new Exception(
                    "Invalid email or password.");
            }

            var token =
                _jwtService.GenerateToken(
                    user.Id,
                    user.Email,
                    user.Role);

            return new LoginResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }
    }
}