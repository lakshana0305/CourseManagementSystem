using CourseManagementSystem.Application.DTOs.Enrollment;
using CourseManagementSystem.Application.Services;
using CourseManagementSystem.Domain.Entities;
using CourseManagementSystem.Infrastructure.Repositories.Interfaces;
using Moq;

namespace CourseManagementSystem.Tests
{
    public class EnrollmentServiceTests
    {
        // TEST 1: Successful enrollment
        [Fact]
        public async Task EnrollAsync_WithValidCourse_CreatesEnrollment()
        {
            // Arrange
            var enrollmentRepository =
                new Mock<IEnrollmentRepository>();

            var courseRepository =
                new Mock<ICourseRepository>();

            var course = new Course
            {
                Id = 1,
                Title = "C#",
                Description = "C# Programming",
                Instructor = "John"
            };

            var dto = new CreateEnrollmentDto
            {
                UserId = 6,
                CourseId = 1
            };

            var createdEnrollment = new Enrollment
            {
                Id = 1,
                UserId = 6,
                CourseId = 1
            };

            courseRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(course);

            enrollmentRepository
                .Setup(x => x.GetByUserAndCourseAsync(6, 1))
                .ReturnsAsync((Enrollment?)null);

            enrollmentRepository
                .Setup(x => x.AddAsync(It.IsAny<Enrollment>()))
                .ReturnsAsync(createdEnrollment);

            var enrollmentService =
                new EnrollmentService(
                    enrollmentRepository.Object,
                    courseRepository.Object);

            // Act
            var result =
                await enrollmentService.EnrollAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(6, result.UserId);
            Assert.Equal(1, result.CourseId);

            enrollmentRepository.Verify(
                x => x.AddAsync(It.IsAny<Enrollment>()),
                Times.Once);
        }


        // TEST 2: Course does not exist
        [Fact]
        public async Task EnrollAsync_WhenCourseDoesNotExist_ThrowsException()
        {
            // Arrange
            var enrollmentRepository =
                new Mock<IEnrollmentRepository>();

            var courseRepository =
                new Mock<ICourseRepository>();

            courseRepository
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Course?)null);

            var enrollmentService =
                new EnrollmentService(
                    enrollmentRepository.Object,
                    courseRepository.Object);

            var dto = new CreateEnrollmentDto
            {
                UserId = 6,
                CourseId = 999
            };

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<Exception>(
                    () => enrollmentService.EnrollAsync(dto));

            Assert.Equal(
                "Course not found.",
                exception.Message);
        }


        // TEST 3: Duplicate enrollment
        [Fact]
        public async Task EnrollAsync_WhenAlreadyEnrolled_ThrowsException()
        {
            // Arrange
            var enrollmentRepository =
                new Mock<IEnrollmentRepository>();

            var courseRepository =
                new Mock<ICourseRepository>();

            var course = new Course
            {
                Id = 1,
                Title = "C#",
                Description = "C# Programming",
                Instructor = "John"
            };

            var existingEnrollment = new Enrollment
            {
                Id = 1,
                UserId = 6,
                CourseId = 1
            };

            courseRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(course);

            enrollmentRepository
                .Setup(x => x.GetByUserAndCourseAsync(6, 1))
                .ReturnsAsync(existingEnrollment);

            var enrollmentService =
                new EnrollmentService(
                    enrollmentRepository.Object,
                    courseRepository.Object);

            var dto = new CreateEnrollmentDto
            {
                UserId = 6,
                CourseId = 1
            };

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<Exception>(
                    () => enrollmentService.EnrollAsync(dto));

            Assert.Equal(
                "Student is already enrolled in this course.",
                exception.Message);
        }


        // TEST 4: Get student's enrollments
        [Fact]
        public async Task GetMyEnrollmentsAsync_ReturnsStudentEnrollments()
        {
            // Arrange
            var enrollmentRepository =
                new Mock<IEnrollmentRepository>();

            var courseRepository =
                new Mock<ICourseRepository>();

            var enrollments = new List<Enrollment>
            {
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
                }
            };

            enrollmentRepository
                .Setup(x => x.GetByUserIdAsync(6))
                .ReturnsAsync(enrollments);

            var enrollmentService =
                new EnrollmentService(
                    enrollmentRepository.Object,
                    courseRepository.Object);

            // Act
            var result =
                await enrollmentService
                    .GetMyEnrollmentsAsync(6);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(6, result[0].UserId);
            Assert.Equal(6, result[1].UserId);
        }


        // TEST 5: Cancel own enrollment
        [Fact]
        public async Task CancelEnrollmentAsync_WhenEnrollmentBelongsToUser_DeletesEnrollment()
        {
            // Arrange
            var enrollmentRepository =
                new Mock<IEnrollmentRepository>();

            var courseRepository =
                new Mock<ICourseRepository>();

            var enrollment = new Enrollment
            {
                Id = 1,
                UserId = 6,
                CourseId = 1
            };

            enrollmentRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(enrollment);

            var enrollmentService =
                new EnrollmentService(
                    enrollmentRepository.Object,
                    courseRepository.Object);

            // Act
            await enrollmentService
                .CancelEnrollmentAsync(1, 6);

            // Assert
            enrollmentRepository.Verify(
                x => x.DeleteAsync(enrollment),
                Times.Once);
        }


        // TEST 6: Cannot cancel another student's enrollment
        [Fact]
        public async Task CancelEnrollmentAsync_WhenEnrollmentBelongsToAnotherUser_ThrowsException()
        {
            // Arrange
            var enrollmentRepository =
                new Mock<IEnrollmentRepository>();

            var courseRepository =
                new Mock<ICourseRepository>();

            var enrollment = new Enrollment
            {
                Id = 1,
                UserId = 6,
                CourseId = 1
            };

            enrollmentRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(enrollment);

            var enrollmentService =
                new EnrollmentService(
                    enrollmentRepository.Object,
                    courseRepository.Object);

            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<Exception>(
                    () => enrollmentService
                        .CancelEnrollmentAsync(1, 10));

            Assert.Equal(
                "You cannot cancel another student's enrollment.",
                exception.Message);
        }
    }
}