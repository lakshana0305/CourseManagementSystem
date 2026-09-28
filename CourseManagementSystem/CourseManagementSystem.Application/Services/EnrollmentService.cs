using CourseManagementSystem.Application.DTOs.Enrollment;
using CourseManagementSystem.Application.Services.Interfaces;
using CourseManagementSystem.Domain.Entities;
using CourseManagementSystem.Infrastructure.Repositories.Interfaces;

namespace CourseManagementSystem.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            ICourseRepository courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
        }

        public async Task<Enrollment> EnrollAsync(
            CreateEnrollmentDto dto)
        {
            // Check whether the course exists
            var course =
                await _courseRepository.GetByIdAsync(dto.CourseId);

            if (course == null)
            {
                throw new Exception("Course not found.");
            }

            // Check whether the student is already enrolled
            var existingEnrollment =
                await _enrollmentRepository
                    .GetByUserAndCourseAsync(
                        dto.UserId,
                        dto.CourseId);

            if (existingEnrollment != null)
            {
                throw new Exception(
                    "Student is already enrolled in this course.");
            }

            // Create new enrollment
            var enrollment = new Enrollment
            {
                UserId = dto.UserId,
                CourseId = dto.CourseId
            };

            return await _enrollmentRepository
                .AddAsync(enrollment);
        }

        public async Task<List<Enrollment>> GetMyEnrollmentsAsync(
            int userId)
        {
            return await _enrollmentRepository
                .GetByUserIdAsync(userId);
        }

        public async Task CancelEnrollmentAsync(
            int enrollmentId,
            int userId)
        {
            // Find the enrollment
            var enrollment =
                await _enrollmentRepository
                    .GetByIdAsync(enrollmentId);

            if (enrollment == null)
            {
                throw new Exception("Enrollment not found.");
            }

            // Ownership check
            if (enrollment.UserId != userId)
            {
                throw new Exception(
                    "You cannot cancel another student's enrollment.");
            }

            // Delete only if the enrollment belongs to the
            // currently authenticated student
            await _enrollmentRepository
                .DeleteAsync(enrollment);
        }
    }
}