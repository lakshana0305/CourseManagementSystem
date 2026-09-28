using CourseManagementSystem.Application.DTOs.User;

namespace CourseManagementSystem.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginUserDto dto);
    }
}