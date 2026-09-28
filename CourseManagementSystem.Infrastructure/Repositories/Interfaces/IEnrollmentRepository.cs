using CourseManagementSystem.Domain.Entities;

namespace CourseManagementSystem.Infrastructure.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment?> GetByIdAsync(int id);

        Task<Enrollment?> GetByUserAndCourseAsync(
            int userId,
            int courseId);

        Task<List<Enrollment>> GetByUserIdAsync(int userId);

        Task<Enrollment> AddAsync(Enrollment enrollment);

        Task DeleteAsync(Enrollment enrollment);
    }
}