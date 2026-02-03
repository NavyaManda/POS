using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Models;
using AuthService.API.Repositories;
using AuthService.API.Data;

namespace AuthService.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<AuthDbContext> _dbContextOptions;

        public UserRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private AuthDbContext CreateContext()
        {
            return new AuthDbContext(_dbContextOptions);
        }

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                PasswordHash = "hashed-password",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true
            };

            using (var context = CreateContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);
                var result = await repository.GetByIdAsync(userId);

                // Assert
                result.Should().NotBeNull();
                result!.Id.Should().Be(userId);
                result.Email.Should().Be("test@example.com");
            }
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetByIdAsync(nonExistingId);

                // Assert
                result.Should().BeNull();
            }
        }

        #endregion

        #region GetByEmailAsync Tests

        [Fact]
        public async Task GetByEmailAsync_WithExistingEmail_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = "hashed-password",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true
            };

            using (var context = CreateContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);
                var result = await repository.GetByEmailAsync(email);

                // Assert
                result.Should().NotBeNull();
                result!.Email.Should().Be(email);
            }
        }

        [Fact]
        public async Task GetByEmailAsync_WithNonExistingEmail_ReturnsNull()
        {
            // Arrange
            var email = "nonexistent@example.com";

            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetByEmailAsync(email);

                // Assert
                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetByEmailAsync_IsCaseInsensitive()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = "hashed-password",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true
            };

            using (var context = CreateContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);
                var result = await repository.GetByEmailAsync("TEST@EXAMPLE.COM");

                // Assert
                result.Should().NotBeNull();
            }
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidUser_ReturnsTrue()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "newuser@example.com",
                PasswordHash = "hashed-password",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true
            };

            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.CreateAsync(user);

                // Assert
                result.Should().BeTrue();
            }

            // Verify
            using (var context = CreateContext())
            {
                var savedUser = await context.Users.FindAsync(user.Id);
                savedUser.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateEmail_ThrowsException()
        {
            // Arrange
            var email = "test@example.com";
            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = "hashed-password-1",
                FirstName = "John",
                LastName = "Doe"
            };

            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = "hashed-password-2",
                FirstName = "Jane",
                LastName = "Smith"
            };

            using (var context = CreateContext())
            {
                context.Users.Add(user1);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);
                await Assert.ThrowsAsync<DbUpdateException>(() => repository.CreateAsync(user2));
            }
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithExistingUser_UpdatesUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                PasswordHash = "hashed-password",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true
            };

            using (var context = CreateContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Modify the user
            user.FirstName = "Jane";
            user.IsActive = false;

            // Act
            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);
                var result = await repository.UpdateAsync(user);

                // Assert
                result.Should().BeTrue();
            }

            // Verify
            using (var context = CreateContext())
            {
                var updatedUser = await context.Users.FindAsync(userId);
                updatedUser.Should().NotBeNull();
                updatedUser!.FirstName.Should().Be("Jane");
                updatedUser.IsActive.Should().BeFalse();
            }
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingUser_ReturnsFalse()
        {
            // Arrange
            var nonExistingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "nonexistent@example.com",
                PasswordHash = "hashed-password",
                FirstName = "John",
                LastName = "Doe"
            };

            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.UpdateAsync(nonExistingUser);

                // Assert
                result.Should().BeFalse();
            }
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithExistingId_DeletesUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                PasswordHash = "hashed-password",
                FirstName = "John",
                LastName = "Doe"
            };

            using (var context = CreateContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);
                var result = await repository.DeleteAsync(userId);

                // Assert
                result.Should().BeTrue();
            }

            // Verify
            using (var context = CreateContext())
            {
                var deletedUser = await context.Users.FindAsync(userId);
                deletedUser.Should().BeNull();
            }
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ReturnsFalse()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            using (var context = CreateContext())
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.DeleteAsync(nonExistingId);

                // Assert
                result.Should().BeFalse();
            }
        }

        #endregion
    }
}
