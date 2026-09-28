using CourseManagementSystem.Application.DTOs.User;
using CourseManagementSystem.Application.Services.Interfaces;
using CourseManagementSystem.Domain.Entities;
using CourseManagementSystem.Infrastructure.Repositories.Interfaces;

namespace CourseManagementSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public UserService(
            IUserRepository userRepository,
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user =
                await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users =
                await _userRepository.GetAllAsync();

            return users.Select(user => new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            }).ToList();
        }

        public async Task<UserResponseDto> RegisterAsync(
            RegisterUserDto dto)
        {
            var existingUser =
                await _userRepository.GetByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                throw new Exception(
                    "Email is already registered.");
            }

            // Hash the password before storing it
            var passwordHash =
                _passwordService.HashPassword(dto.Password);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = "Student"
            };

            var createdUser =
                await _userRepository.AddAsync(user);

            return new UserResponseDto
            {
                Id = createdUser.Id,
                Name = createdUser.Name,
                Email = createdUser.Email,
                Role = createdUser.Role
            };
        }

        public async Task UpdateAsync(
            int id,
            UpdateUserDto dto)
        {
            var existingUser =
                await _userRepository.GetByIdAsync(id);

            if (existingUser == null)
            {
                throw new Exception("User not found.");
            }

            existingUser.Name = dto.Name;
            existingUser.Email = dto.Email;
            existingUser.Role = dto.Role;

            await _userRepository.UpdateAsync(existingUser);
        }

        public async Task DeleteAsync(int id)
        {
            var user =
                await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            await _userRepository.DeleteAsync(user);
        }
    }
}