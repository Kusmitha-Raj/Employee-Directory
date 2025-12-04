using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeDirectoryApp.Data;
using EmployeeDirectoryApp.DTO;
using EmployeeDirectoryApp.Models.Entities;
using EmployeeDirectoryApp.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeDirectoryTest.Services
{
    public class UserServiceTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private UserService CreateService(AppDbContext context)
        {
            return new UserService(context);
        }

        [Fact]
        public async Task ValidateUserAsync_ReturnsUser_WhenCredentialsAreValid()
        {
            // Arrange
            using var context = CreateContext();
            var password = "Test@123";
            var user = new User
            {
                Email = "user@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Employee",
                MustChangePassword = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            // Act
            var result = await service.ValidateUserAsync("user@test.com", password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user@test.com", result!.Email);
        }

        [Fact]
        public async Task ValidateUserAsync_ReturnsNull_WhenPasswordIsInvalid()
        {
            using var context = CreateContext();
            var user = new User
            {
                Email = "user@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Correct@123"),
                Role = "Employee",
                MustChangePassword = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.ValidateUserAsync("user@test.com", "Wrong@123");

            Assert.Null(result);
        }

        [Fact]
        public async Task ValidateUserAsync_ReturnsNull_WhenUserNotFound()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var result = await service.ValidateUserAsync("missing@test.com", "anything");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsUser_WhenExists()
        {
            using var context = CreateContext();
            var user = new User
            {
                Email = "user@test.com",
                PasswordHash = "hash",
                Role = "Employee",
                MustChangePassword = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetByEmailAsync("user@test.com");

            Assert.NotNull(result);
            Assert.Equal("user@test.com", result!.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsNull_WhenNotExists()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var result = await service.GetByEmailAsync("missing@test.com");

            Assert.Null(result);
        }

        [Fact]
        public async Task AddEmployeeAsync_CreatesUserAndEmployee_WhenBothDoNotExist()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var dto = new CreateEmployeeDto
            {
                FirstName = "Raksha",
                LastName = "Achary",
                Email = "raksha@test.com",
                Gender = "Female",
                JobRole = "Developer",
                DepartmentId = 1,
                PhoneNo = "9999999999"
            };

            var result = await service.AddEmployeeAsync(dto);

            Assert.True(result);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            var employee = await context.Employees.FirstOrDefaultAsync(e => e.Email == dto.Email);

            Assert.NotNull(user);
            Assert.NotNull(employee);
            Assert.Equal(user!.UserId, employee!.UserId);
            Assert.Equal("Employee", user.Role);
            Assert.True(user.MustChangePassword);
            Assert.True(BCrypt.Net.BCrypt.Verify(dto.FirstName + "@123", user.PasswordHash));
        }

        [Fact]
        public async Task AddEmployeeAsync_ReusesExistingUser_WhenUserExistsButEmployeeDoesNot()
        {
            using var context = CreateContext();

            var existingUser = new User
            {
                Email = "kavana@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Old@123"),
                Role = "Employee",
                MustChangePassword = false
            };
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new CreateEmployeeDto
            {
                FirstName = "Kavana",
                LastName = "K",
                Email = "kavana@test.com",
                Gender = "Female",
                JobRole = "Engineer",
                DepartmentId = 2,
                PhoneNo = "8888888888"
            };

            var result = await service.AddEmployeeAsync(dto);

            Assert.True(result);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            var employee = await context.Employees.FirstOrDefaultAsync(e => e.Email == dto.Email);

            Assert.NotNull(user);
            Assert.NotNull(employee);
            Assert.Equal(existingUser.UserId, user!.UserId);
            Assert.Equal(existingUser.UserId, employee!.UserId);
        }

        [Fact]
        public async Task AddAdminAsync_CreatesAdmin_WhenNotExists()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var dto = new CreateAdminDto
            {
                Email = "admin@test.com"
            };

            var result = await service.AddAdminAsync(dto);

            Assert.True(result);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            Assert.NotNull(user);
            Assert.Equal("Admin", user!.Role);
            Assert.True(user.MustChangePassword);
            Assert.True(BCrypt.Net.BCrypt.Verify("Admin@123", user.PasswordHash));
        }

        [Fact]
        public async Task AddAdminAsync_ReturnsFalse_WhenUserAlreadyExists()
        {
            using var context = CreateContext();

            var existingUser = new User
            {
                Email = "admin@test.com",
                PasswordHash = "hash",
                Role = "Admin",
                MustChangePassword = false
            };
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new CreateAdminDto
            {
                Email = "admin@test.com"
            };

            var result = await service.AddAdminAsync(dto);

            Assert.False(result);
            Assert.Equal(1, await context.Users.CountAsync());
        }

        [Fact]
        public async Task GetAllAsync_ReturnsUserDtos()
        {
            using var context = CreateContext();

            context.Users.AddRange(
                new User
                {
                    Email = "user1@test.com",
                    PasswordHash = "hash1",
                    Role = "Employee",
                    MustChangePassword = false
                },
                new User
                {
                    Email = "user2@test.com",
                    PasswordHash = "hash2",
                    Role = "Admin",
                    MustChangePassword = false
                }
            );
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Email == "user1@test.com" && u.Role == "Employee");
            Assert.Contains(result, u => u.Email == "user2@test.com" && u.Role == "Admin");
        }

        [Fact]
        public async Task GetRefreshTokenAsync_ReturnsToken_WhenExists()
        {
            using var context = CreateContext();

            var user = new User
            {
                Email = "user@test.com",
                PasswordHash = "hash",
                Role = "Employee",
                MustChangePassword = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var token = new RefreshTokens
            {
                Token = "refresh-token-123",
                User = user
            };
            context.RefreshTokens.Add(token);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetRefreshTokenAsync("refresh-token-123");

            Assert.NotNull(result);
            Assert.Equal("refresh-token-123", result!.Token);
            Assert.NotNull(result.User);
            Assert.Equal("user@test.com", result.User!.Email);
        }

        [Fact]
        public async Task GetRefreshTokenAsync_ReturnsNull_WhenNotExists()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var result = await service.GetRefreshTokenAsync("missing-token");

            Assert.Null(result);
        }

        [Fact]
        public async Task SaveRefreshTokenAsync_AddsToken()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var user = new User
            {
                Email = "user@test.com",
                PasswordHash = "hash",
                Role = "Employee",
                MustChangePassword = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var token = new RefreshTokens
            {
                Token = "new-token",
                User = user
            };

            var result = await service.SaveRefreshTokenAsync(token);

            Assert.True(result);
            Assert.Equal(1, await context.RefreshTokens.CountAsync());
        }

        [Fact]
        public async Task UpdateRefreshTokenAsync_UpdatesToken()
        {
            using var context = CreateContext();

            var user = new User
            {
                Email = "user@test.com",
                PasswordHash = "hash",
                Role = "Employee",
                MustChangePassword = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var token = new RefreshTokens
            {
                Token = "old-token",
                User = user
            };
            context.RefreshTokens.Add(token);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            token.Token = "updated-token";

            var result = await service.UpdateRefreshTokenAsync(token);

            Assert.True(result);

            var updated = await context.RefreshTokens.FirstOrDefaultAsync();
            Assert.NotNull(updated);
            Assert.Equal("updated-token", updated!.Token);
        }
    }
}
