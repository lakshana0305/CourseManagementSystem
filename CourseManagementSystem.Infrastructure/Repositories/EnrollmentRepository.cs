using CourseManagementSystem.Infrastructure.Data;
using CourseManagementSystem.Domain.Entities;
using CourseManagementSystem.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Infrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Enrollment?> GetByUserAndCourseAsync(
            int userId,
            int courseId)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId &&
                    e.CourseId == courseId);
        }

        public async Task<List<Enrollment>> GetByUserIdAsync(int userId)
        {
            return await _context.Enrollments
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        public async Task<Enrollment> AddAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return enrollment;
        }

        public async Task DeleteAsync(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}