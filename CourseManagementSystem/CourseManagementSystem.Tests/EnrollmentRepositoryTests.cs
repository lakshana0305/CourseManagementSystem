using CourseManagementSystem.Domain.Entities;
using CourseManagementSystem.Infrastructure.Data;
using CourseManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Tests
{
    public class EnrollmentRepositoryTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetByIdAsync_WhenEnrollmentExists_ReturnsEnrollment()
        {
            // Arrange
            using var context = CreateContext();

            var enrollment = new Enrollment
            {
                Id = 1,
                UserId = 6,
                CourseId = 2
            };

            context.Enrollments.Add(enrollment);
            await context.SaveChangesAsync();

            var repository = new EnrollmentRepository(context);

            // Act
            var result =
                await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(6, result.UserId);
            Assert.Equal(2, result.CourseId);
        }

        [Fact]
        public async Task GetByUserAndCourseAsync_WhenEnrollmentExists_ReturnsEnrollment()
        {
            // Arrange
            using var context = CreateContext();

            var enrollment = new Enrollment
            {
                Id = 1,
                UserId = 6,
                CourseId = 2
            };

            context.Enrollments.Add(enrollment);
            await context.SaveChangesAsync();

            var repository = new EnrollmentRepository(context);

            // Act
            var result =
                await repository.GetByUserAndCourseAsync(6, 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.UserId);
            Assert.Equal(2, result.CourseId);
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsUserEnrollments()
        {
            // Arrange
            using var context = CreateContext();

            context.Enrollments.AddRange(
                new Enrollment
                {
                    Id = 1,
                    UserId = 6,
                    CourseId = 1
                },
                new Enrollment
                {
                    Id = 2,
                    UserId = 6,
                    CourseId = 2
                },
                new Enrollment
                {
                    Id = 3,
                    UserId = 10,
                    CourseId = 1
                });

            await context.SaveChangesAsync();

            var repository = new EnrollmentRepository(context);

            // Act
            var result =
                await repository.GetByUserIdAsync(6);

            // Assert
            Assert.Equal(2, result.Count);

            Assert.All(
                result,
                enrollment =>
                    Assert.Equal(6, enrollment.UserId));
        }
    }
}