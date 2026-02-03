using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using AuthService.API.Services;
using AuthService.API.Models;
using AuthService.API.Interfaces;
using BCrypt.Net;

namespace AuthService.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthService.API.Services.AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _mockTokenService = new Mock<ITokenService>();

            _authService = new AuthService.API.Services.AuthService(
                _mockUserRepository.Object,
                _mockRefreshTokenRepository.Object,
                _mockTokenService.Object
            );
        }

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var email = "test@example.com";
            var password = "Password123!";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = email,
                PasswordHash = passwordHash,
                FirstName = "John",
                LastName = "Doe",
                IsActive = true,
                UserRoles = new List<UserRole>()
            };

            var loginRequest = new LoginRequest { Email = email, Password = password };
            var accessToken = "access-token-123";
            var refreshToken = "refresh-token-456";

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);
            _mockTokenService.Setup(x => x.GenerateAccessToken(userId.ToString(), email, It.IsAny<List<string>>()))
                .Returns(accessToken);
            _mockTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns(refreshToken);
            _mockRefreshTokenRepository.Setup(x => x.CreateAsync(It.IsAny<RefreshToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be(accessToken);
            result.RefreshToken.Should().Be(refreshToken);
            result.UserId.Should().Be(userId);
            result.Email.Should().Be(email);
            _mockUserRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidEmail_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "nonexistent@example.com", Password = "Password123!" };
            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginRequest));
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var email = "test@example.com";
            var password = "WrongPassword123!";
            var correctPassword = "CorrectPassword123!";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = passwordHash,
                FirstName = "John",
                LastName = "Doe",
                IsActive = true,
                UserRoles = new List<UserRole>()
            };

            var loginRequest = new LoginRequest { Email = email, Password = password };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginRequest));
        }

        [Fact]
        public async Task LoginAsync_WithInactiveUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var email = "test@example.com";
            var password = "Password123!";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = passwordHash,
                FirstName = "John",
                LastName = "Doe",
                IsActive = false, // Inactive user
                UserRoles = new List<UserRole>()
            };

            var loginRequest = new LoginRequest { Email = email, Password = password };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginRequest));
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_UpdatesLastLoginTime()
        {
            // Arrange
            var email = "test@example.com";
            var password = "Password123!";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = email,
                PasswordHash = passwordHash,
                FirstName = "John",
                LastName = "Doe",
                IsActive = true,
                LastLoginAt = null,
                UserRoles = new List<UserRole>()
            };

            var loginRequest = new LoginRequest { Email = email, Password = password };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);
            _mockTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                .Returns("token");
            _mockTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh");
            _mockRefreshTokenRepository.Setup(x => x.CreateAsync(It.IsAny<RefreshToken>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            // Act
            await _authService.LoginAsync(loginRequest);

            // Assert
            _mockUserRepository.Verify(x => x.UpdateAsync(It.Is<User>(u => u.LastLoginAt != null)), Times.Once);
        }

        #endregion

        #region RegisterAsync Tests

        [Fact]
        public async Task RegisterAsync_WithValidRequest_CreatesNewUser()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "newuser@example.com",
                Password = "ValidPassword123!",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            // Act
            await _authService.RegisterAsync(registerRequest);

            // Assert
            _mockUserRepository.Verify(
                x => x.CreateAsync(It.Is<User>(u =>
                    u.Email == registerRequest.Email &&
                    u.FirstName == registerRequest.FirstName &&
                    u.LastName == registerRequest.LastName)),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var email = "existing@example.com";
            var registerRequest = new RegisterRequest
            {
                Email = email,
                Password = "ValidPassword123!",
                FirstName = "John",
                LastName = "Doe"
            };

            var existingUser = new User { Id = Guid.NewGuid(), Email = email };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(registerRequest));
        }

        [Fact]
        public async Task RegisterAsync_PasswordIsHashed_DoesNotStorePlainPassword()
        {
            // Arrange
            var plainPassword = "ValidPassword123!";
            var registerRequest = new RegisterRequest
            {
                Email = "newuser@example.com",
                Password = plainPassword,
                FirstName = "John",
                LastName = "Doe"
            };

            User capturedUser = null;
            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync(true);

            // Act
            await _authService.RegisterAsync(registerRequest);

            // Assert
            capturedUser.Should().NotBeNull();
            capturedUser!.PasswordHash.Should().NotBe(plainPassword);
            capturedUser.PasswordHash.Should().NotBeNullOrEmpty();
            BCrypt.Net.BCrypt.Verify(plainPassword, capturedUser.PasswordHash).Should().BeTrue();
        }

        #endregion

        #region RefreshTokenAsync Tests

        [Fact]
        public async Task RefreshTokenAsync_WithValidRefreshToken_ReturnsNewAccessToken()
        {
            // Arrange
            var refreshTokenValue = "valid-refresh-token";
            var userId = Guid.NewGuid();
            var email = "test@example.com";

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = refreshTokenValue,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            var user = new User
            {
                Id = userId,
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                UserRoles = new List<UserRole>()
            };

            var newAccessToken = "new-access-token";

            _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync(refreshTokenValue))
                .ReturnsAsync(refreshToken);
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockTokenService.Setup(x => x.GenerateAccessToken(userId.ToString(), email, It.IsAny<List<string>>()))
                .Returns(newAccessToken);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshTokenValue);

            // Assert
            result.AccessToken.Should().Be(newAccessToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_WithInvalidToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var refreshTokenValue = "invalid-token";

            _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync(refreshTokenValue))
                .ReturnsAsync((RefreshToken)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.RefreshTokenAsync(refreshTokenValue));
        }

        [Fact]
        public async Task RefreshTokenAsync_WithExpiredToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var refreshTokenValue = "expired-token";

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Token = refreshTokenValue,
                ExpiryDate = DateTime.UtcNow.AddDays(-1), // Already expired
                IsRevoked = false
            };

            _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync(refreshTokenValue))
                .ReturnsAsync(refreshToken);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.RefreshTokenAsync(refreshTokenValue));
        }

        [Fact]
        public async Task RefreshTokenAsync_WithRevokedToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var refreshTokenValue = "revoked-token";

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Token = refreshTokenValue,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = true
            };

            _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync(refreshTokenValue))
                .ReturnsAsync(refreshToken);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.RefreshTokenAsync(refreshTokenValue));
        }

        #endregion

        #region ValidateTokenAsync Tests

        [Fact]
        public async Task ValidateTokenAsync_WithValidToken_ReturnsValidationResponse()
        {
            // Arrange
            var token = "valid-token";
            var userId = Guid.NewGuid().ToString();
            var email = "test@example.com";

            var tokenClaims = new TokenClaims
            {
                UserId = userId,
                Email = email,
                Roles = new List<string> { "Admin" }
            };

            _mockTokenService.Setup(x => x.ValidateToken(token))
                .Returns(tokenClaims);

            // Act
            var result = await _authService.ValidateTokenAsync(token);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.UserId.Should().Be(userId);
            result.Email.Should().Be(email);
        }

        [Fact]
        public async Task ValidateTokenAsync_WithInvalidToken_ReturnsFalse()
        {
            // Arrange
            var token = "invalid-token";

            _mockTokenService.Setup(x => x.ValidateToken(token))
                .Throws<Exception>();

            // Act
            var result = await _authService.ValidateTokenAsync(token);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        #endregion
    }
}
