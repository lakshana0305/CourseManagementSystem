using CourseManagementSystem.Domain.Entities;
using CourseManagementSystem.Infrastructure.Data;
using CourseManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Tests
{
    public class CourseRepositoryTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCourses()
        {
            // Arrange
            using var context = CreateContext();

            context.Courses.AddRange(
                new Course
                {
                    Id = 1,
                    Title = "C#",
                    Description = "C# Programming",
                    Instructor = "John"
                },
                new Course
                {
                    Id = 2,
                    Title = "ASP.NET Core",
                    Description = "Web API Development",
                    Instructor = "David"
                },
                new Course
                {
                    Id = 3,
                    Title = "SQL Server",
                    Description = "Database Management",
                    Instructor = "Robert"
                });

            await context.SaveChangesAsync();

            var repository = new CourseRepository(context);

            // Act
            var result =
                await repository.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count);

            Assert.Contains(
                result,
                course => course.Title == "C#");

            Assert.Contains(
                result,
                course => course.Title == "ASP.NET Core");

            Assert.Contains(
                result,
                course => course.Title == "SQL Server");
        }

        [Fact]
        public async Task GetByIdAsync_WhenCourseExists_ReturnsCourse()
        {
            // Arrange
            using var context = CreateContext();

            var course = new Course
            {
                Id = 1,
                Title = "C#",
                Description = "C# Programming",
                Instructor = "John"
            };

            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var repository = new CourseRepository(context);

            // Act
            var result =
                await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("C#", result.Title);
            Assert.Equal(
                "C# Programming",
                result.Description);
            Assert.Equal(
                "John",
                result.Instructor);
        }
    }
}