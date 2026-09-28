using CourseManagementSystem.Domain.Entities;
using CourseManagementSystem.Infrastructure.Data;
using CourseManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Tests
{
    public class UserRepositoryTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetByEmailAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            using var context = CreateContext();

            var user = new User
            {
                Id = 1,
                Name = "Student Test",
                Email = "student@gmail.com",
                PasswordHash = "hashed-password",
                Role = "Student"
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result =
                await repository.GetByEmailAsync(
                    "student@gmail.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(
                "student@gmail.com",
                result.Email);
            Assert.Equal(
                "Student",
                result.Role);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            using var context = CreateContext();

            var user = new User
            {
                Id = 1,
                Name = "Student Test",
                Email = "student@gmail.com",
                PasswordHash = "hashed-password",
                Role = "Student"
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result =
                await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(
                "Student Test",
                result.Name);
            Assert.Equal(
                "student@gmail.com",
                result.Email);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllUsers()
        {
            // Arrange
            using var context = CreateContext();

            context.Users.AddRange(
                new User
                {
                    Id = 1,
                    Name = "Student One",
                    Email = "student1@gmail.com",
                    PasswordHash = "hash1",
                    Role = "Student"
                },
                new User
                {
                    Id = 2,
                    Name = "Student Two",
                    Email = "student2@gmail.com",
                    PasswordHash = "hash2",
                    Role = "Student"
                },
                new User
                {
                    Id = 3,
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    PasswordHash = "hash3",
                    Role = "Admin"
                });

            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result =
                await repository.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(
                result,
                user => user.Email == "student1@gmail.com");
            Assert.Contains(
                result,
                user => user.Email == "student2@gmail.com");
            Assert.Contains(
                result,
                user => user.Email == "admin@gmail.com");
        }
    }
}