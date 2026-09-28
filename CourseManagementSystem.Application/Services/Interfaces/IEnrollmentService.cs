using CourseManagementSystem.Application.DTOs.Enrollment;
using CourseManagementSystem.Domain.Entities;

namespace CourseManagementSystem.Application.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<Enrollment> EnrollAsync(CreateEnrollmentDto dto);

        Task<List<Enrollment>> GetMyEnrollmentsAsync(int userId);

        Task CancelEnrollmentAsync(
            int enrollmentId,
            int userId);
    }
}