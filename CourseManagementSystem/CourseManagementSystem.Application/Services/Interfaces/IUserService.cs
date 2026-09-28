using CourseManagementSystem.Application.DTOs.User;

namespace CourseManagementSystem.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto?> GetByIdAsync(int id);

        Task<List<UserResponseDto>> GetAllAsync();

        Task<UserResponseDto> RegisterAsync(RegisterUserDto dto);

        Task UpdateAsync(int id, UpdateUserDto dto);

        Task DeleteAsync(int id);
    }
}