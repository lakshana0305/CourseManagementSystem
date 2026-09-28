using CourseManagementSystem.Application.DTOs.User;
using CourseManagementSystem.Application.Services;
using CourseManagementSystem.Application.Services.Interfaces;
using CourseManagementSystem.Domain.Entities;
using CourseManagementSystem.Infrastructure.Repositories.Interfaces;
using Moq;

namespace CourseManagementSystem.Tests
{
    public class UserServiceTests
    {
        // TEST 1: Get user by ID
        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var passwordService = new Mock<IPasswordService>();

            var user = new User
            {
                Id = 6,
                Name = "Student Test",
                Email = "studenttest@gmail.com",
                PasswordHash = "hashed-password",
                Role = "Student"
            };

            userRepository
                .Setup(x => x.GetByIdAsync(6))
                .ReturnsAsync(user);

            var userService = new UserService(
                userRepository.Object,
                passwordService.Object);

            // Act
            var result = await userService.GetByIdAsync(6);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Id);
            Assert.Equal("Student Test", result.Name);
            Assert.Equal("studenttest@gmail.com", result.Email);
            Assert.Equal("Student", result.Role);
        }


        // TEST 2: Get user by ID when user doesn't exist
        [Fact]
        public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var passwordService = new Mock<IPasswordService>();

            userRepository
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((User?)null);

            var userService = new UserService(
                userRepository.Object,
                passwordService.Object);

            // Act
            var result = await userService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }


        // TEST 3: Register new user
        [Fact]
        public async Task RegisterAsync_WithNewEmail_CreatesUser()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var passwordService = new Mock<IPasswordService>();

            var dto = new RegisterUserDto
            {
                Name = "New Student",
                Email = "newstudent@gmail.com",
                Password = "Student@123"
            };

            userRepository
                .Setup(x => x.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            passwordService
                .Setup(x => x.HashPassword(dto.Password))
                .Returns("hashed-password");

            var createdUser = new User
            {
                Id = 10,
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = "hashed-password",
                Role = "Student"
            };

            userRepository
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(createdUser);

            var userService = new UserService(
                userRepository.Object,
                passwordService.Object);

            // Act
            var result = await userService.RegisterAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("New Student", result.Name);
            Assert.Equal("newstudent@gmail.com", result.Email);
            Assert.Equal("Student", result.Role);
        }


        // TEST 4: Register with existing email
        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsException()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var passwordService = new Mock<IPasswordService>();

            var existingUser = new User
            {
                Id = 6,
                Name = "Existing Student",
                Email = "studenttest@gmail.com",
                PasswordHash = "hashed-password",
                Role = "Student"
            };

            userRepository
                .Setup(x => x.GetByEmailAsync(existingUser.Email))
                .ReturnsAsync(existingUser);

            var userService = new UserService(
                userRepository.Object,
                passwordService.Object);

            var dto = new RegisterUserDto
            {
                Name = "Another Student",
                Email = "studenttest@gmail.com",
                Password = "Student@123"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => userService.RegisterAsync(dto));

            Assert.Equal(
                "Email is already registered.",
                exception.Message);
        }


        // TEST 5: Update existing user
        [Fact]
        public async Task UpdateAsync_WhenUserExists_UpdatesUser()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var passwordService = new Mock<IPasswordService>();

            var existingUser = new User
            {
                Id = 6,
                Name = "Old Name",
                Email = "old@gmail.com",
                PasswordHash = "hashed-password",
                Role = "Student"
            };

            userRepository
                .Setup(x => x.GetByIdAsync(6))
                .ReturnsAsync(existingUser);

            var dto = new UpdateUserDto
            {
                Name = "Updated Name",
                Email = "updated@gmail.com",
                Role = "Student"
            };

            var userService = new UserService(
                userRepository.Object,
                passwordService.Object);

            // Act
            await userService.UpdateAsync(6, dto);

            // Assert
            Assert.Equal("Updated Name", existingUser.Name);
            Assert.Equal("updated@gmail.com", existingUser.Email);
            Assert.Equal("Student", existingUser.Role);

            userRepository.Verify(
                x => x.UpdateAsync(existingUser),
                Times.Once);
        }


        // TEST 6: Delete existing user
        [Fact]
        public async Task DeleteAsync_WhenUserExists_DeletesUser()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var passwordService = new Mock<IPasswordService>();

            var user = new User
            {
                Id = 6,
                Name = "Student Test",
                Email = "studenttest@gmail.com",
                PasswordHash = "hashed-password",
                Role = "Student"
            };

            userRepository
                .Setup(x => x.GetByIdAsync(6))
                .ReturnsAsync(user);

            var userService = new UserService(
                userRepository.Object,
                passwordService.Object);

            // Act
            await userService.DeleteAsync(6);

            // Assert
            userRepository.Verify(
                x => x.DeleteAsync(user),
                Times.Once);
        }
    }
}