using CourseManagementSystem.Application.DTOs.Course;
using CourseManagementSystem.Application.Services;
using CourseManagementSystem.Infrastructure.Repositories.Interfaces;
using CourseManagementSystem.Domain.Entities;
using Moq;

namespace CourseManagementSystem.Tests
{
    public class CourseServiceTests
    {
        // TEST 1: Get all courses
        [Fact]
        public async Task GetAllAsync_ReturnsAllCourses()
        {
            // Arrange
            var courseRepository = new Mock<ICourseRepository>();

            var courses = new List<Course>
            {
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
                }
            };

            courseRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(courses);

            var courseService = new CourseService(
                courseRepository.Object);

            // Act
            var result = await courseService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("C#", result[0].Title);
            Assert.Equal("ASP.NET Core", result[1].Title);
        }


        // TEST 2: Get course by ID
        [Fact]
        public async Task GetByIdAsync_WhenCourseExists_ReturnsCourse()
        {
            // Arrange
            var courseRepository = new Mock<ICourseRepository>();

            var course = new Course
            {
                Id = 1,
                Title = "C#",
                Description = "C# Programming",
                Instructor = "John"
            };

            courseRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(course);

            var courseService = new CourseService(
                courseRepository.Object);

            // Act
            var result = await courseService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("C#", result.Title);
            Assert.Equal("C# Programming", result.Description);
            Assert.Equal("John", result.Instructor);
        }


        // TEST 3: Add course
        [Fact]
        public async Task AddAsync_CreatesAndReturnsCourse()
        {
            // Arrange
            var courseRepository = new Mock<ICourseRepository>();

            var dto = new CreateCourseDto
            {
                Title = "Java",
                Description = "Java Programming",
                Instructor = "Robert"
            };

            var createdCourse = new Course
            {
                Id = 3,
                Title = dto.Title,
                Description = dto.Description,
                Instructor = dto.Instructor
            };

            courseRepository
                .Setup(x => x.AddAsync(It.IsAny<Course>()))
                .ReturnsAsync(createdCourse);

            var courseService = new CourseService(
                courseRepository.Object);

            // Act
            var result = await courseService.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.Equal("Java", result.Title);
            Assert.Equal("Java Programming", result.Description);
            Assert.Equal("Robert", result.Instructor);

            courseRepository.Verify(
                x => x.AddAsync(It.IsAny<Course>()),
                Times.Once);
        }


        // TEST 4: Update course
        [Fact]
        public async Task UpdateAsync_WhenCourseExists_UpdatesCourse()
        {
            // Arrange
            var courseRepository = new Mock<ICourseRepository>();

            var existingCourse = new Course
            {
                Id = 1,
                Title = "Old C#",
                Description = "Old Description",
                Instructor = "Old Instructor"
            };

            courseRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingCourse);

            var dto = new UpdateCourseDto
            {
                Title = "Updated C#",
                Description = "Updated Description",
                Instructor = "Updated Instructor"
            };

            var courseService = new CourseService(
                courseRepository.Object);

            // Act
            await courseService.UpdateAsync(1, dto);

            // Assert
            Assert.Equal("Updated C#", existingCourse.Title);
            Assert.Equal(
                "Updated Description",
                existingCourse.Description);
            Assert.Equal(
                "Updated Instructor",
                existingCourse.Instructor);

            courseRepository.Verify(
                x => x.UpdateAsync(existingCourse),
                Times.Once);
        }


        // TEST 5: Delete course
        [Fact]
        public async Task DeleteAsync_WhenCourseExists_DeletesCourse()
        {
            // Arrange
            var courseRepository = new Mock<ICourseRepository>();

            var course = new Course
            {
                Id = 1,
                Title = "C#",
                Description = "C# Programming",
                Instructor = "John"
            };

            courseRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(course);

            var courseService = new CourseService(
                courseRepository.Object);

            // Act
            await courseService.DeleteAsync(1);

            // Assert
            courseRepository.Verify(
                x => x.DeleteAsync(course),
                Times.Once);
        }
    }
}