using CourseManagementSystem.Application.DTOs.User;
using CourseManagementSystem.Application.Services;
using CourseManagementSystem.Application.Services.Interfaces;
using CourseManagementSystem.Domain.Entities;
using CourseManagementSystem.Infrastructure.Repositories.Interfaces;
using Moq;

namespace CourseManagementSystem.Tests
{
    public class AuthServiceTests
    {
        // TEST 1: Valid email and password
        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var passwordService = new Mock<IPasswordService>();
            var jwtService = new Mock<IJwtService>();

            var user = new User
            {
                Id = 6,
                Name = "Student Test",
                Email = "studenttest@gmail.com",
                PasswordHash = "hashed-password",
                Role = "Student"
            };

            userRepository
                .Setup(x => x.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            passwordService
                .Setup(x => x.VerifyPassword(
                    "Student@123",
                    user.PasswordHash))
                .Returns(true);

            jwtService
                .Setup(x => x.GenerateToken(
                    user.Id,
                    user.Email,
                    user.Role))
                .Returns("fake-jwt-token");

            var authService = new AuthService(
                userRepository.Object,
                passwordService.Object,
                jwtService.Object);

            var dto = new LoginUserDto
            {
                Email = user.Email,
                Password = "Student@123"
            };

            // Act
            var result = await authService.LoginAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.UserId);
            Assert.Equal("Student Test", result.Name);
            Assert.Equal("studenttest@gmail.com", result.Email);
            Assert.Equal("Student", result.Role);
            Assert.Equal("fake-jwt-token", result.Token);
        }


        // TEST 2: User does not exist
        [Fact]
        public async Task LoginAsync_WhenUserDoesNotExist_ThrowsException()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var passwordService = new Mock<IPasswordService>();
            var jwtService = new Mock<IJwtService>();

            userRepository
                .Setup(x => x.GetByEmailAsync("unknown@gmail.com"))
                .ReturnsAsync((User?)null);

            var authService = new AuthService(
                userRepository.Object,
                passwordService.Object,
                jwtService.Object);

            var dto = new LoginUserDto
            {
                Email = "unknown@gmail.com",
                Password = "WrongPassword"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => authService.LoginAsync(dto));

            Assert.Equal(
                "Invalid email or password.",
                exception.Message);
        }


        // TEST 3: Wrong password
        [Fact]
        public async Task LoginAsync_WhenPasswordIsWrong_ThrowsException()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var passwordService = new Mock<IPasswordService>();
            var jwtService = new Mock<IJwtService>();

            var user = new User
            {
                Id = 6,
                Name = "Student Test",
                Email = "studenttest@gmail.com",
                PasswordHash = "hashed-password",
                Role = "Student"
            };

            userRepository
                .Setup(x => x.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            passwordService
                .Setup(x => x.VerifyPassword(
                    "WrongPassword",
                    user.PasswordHash))
                .Returns(false);

            var authService = new AuthService(
                userRepository.Object,
                passwordService.Object,
                jwtService.Object);

            var dto = new LoginUserDto
            {
                Email = user.Email,
                Password = "WrongPassword"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => authService.LoginAsync(dto));

            Assert.Equal(
                "Invalid email or password.",
                exception.Message);
        }
    }
}