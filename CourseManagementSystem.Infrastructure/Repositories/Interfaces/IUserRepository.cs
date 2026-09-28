using CourseManagementSystem.Domain.Entities;

namespace CourseManagementSystem.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByIdAsync(int id);

        Task<List<User>> GetAllAsync();

        Task<User> AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(User user);
    }
}