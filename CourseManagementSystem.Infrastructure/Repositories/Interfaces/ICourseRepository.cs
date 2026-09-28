using CourseManagementSystem.Domain.Entities;

namespace CourseManagementSystem.Infrastructure.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int id);
        Task<Course> AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(Course course);
    }
}