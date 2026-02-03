using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuthService.API.Controllers;
using AuthService.API.Models;
using AuthService.API.Interfaces;

namespace AuthService.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        }

        #region Login Tests

        [Fact]
        public async Task Login_WithValidRequest_ReturnsOkResultWithToken()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var loginResponse = new LoginResponse
            {
                UserId = Guid.NewGuid(),
                Email = loginRequest.Email,
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                ExpiresIn = 900
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginRequest))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.StatusCode.Should().Be(200);

            var response = okResult.Value as ApiResponse<LoginResponse>;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.AccessToken.Should().Be("access-token");
        }

        [Fact]
        public async Task Login_WithInvalidEmail_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "invalid@example.com",
                Password = "Password123!"
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginRequest))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            unauthorizedResult.StatusCode.Should().Be(401);

            var response = unauthorizedResult.Value as ApiResponse;
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response.Message.Should().Contain("Invalid email or password");
        }

        [Fact]
        public async Task Login_WithServiceException_ReturnsInternalServerError()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginRequest))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(500);

            var response = statusCodeResult.Value as ApiResponse;
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Login_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "invalid-email",
                Password = "" // Empty password
            };

            _controller.ModelState.AddModelError("Email", "Invalid email format");
            _controller.ModelState.AddModelError("Password", "Password is required");

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.StatusCode.Should().Be(400);

            var response = badRequestResult.Value as ApiResponse;
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response.Errors.Should().NotBeEmpty();
        }

        #endregion

        #region Register Tests

        [Fact]
        public async Task Register_WithValidRequest_ReturnsCreatedResult()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "newuser@example.com",
                Password = "ValidPassword123!",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockAuthService.Setup(x => x.RegisterAsync(registerRequest))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            createdResult.StatusCode.Should().Be(201);

            var response = createdResult.Value as ApiResponse;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Message.Should().Contain("registered successfully");
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "existing@example.com",
                Password = "ValidPassword123!",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockAuthService.Setup(x => x.RegisterAsync(registerRequest))
                .ThrowsAsync(new InvalidOperationException("User already exists"));

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.StatusCode.Should().Be(400);

            var response = badRequestResult.Value as ApiResponse;
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Register_WithWeakPassword_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "newuser@example.com",
                Password = "weak", // Too short
                FirstName = "John",
                LastName = "Doe"
            };

            _controller.ModelState.AddModelError("Password", "Password must be at least 8 characters");

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Register_WithServiceException_ReturnsInternalServerError()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "newuser@example.com",
                Password = "ValidPassword123!",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockAuthService.Setup(x => x.RegisterAsync(registerRequest))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(500);
        }

        #endregion

        #region RefreshToken Tests

        [Fact]
        public async Task RefreshToken_WithValidToken_ReturnsNewAccessToken()
        {
            // Arrange
            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "valid-refresh-token"
            };

            var loginResponse = new LoginResponse
            {
                UserId = Guid.NewGuid(),
                Email = "test@example.com",
                AccessToken = "new-access-token",
                RefreshToken = "new-refresh-token",
                ExpiresIn = 900
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenRequest.RefreshToken))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.RefreshToken(refreshTokenRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.StatusCode.Should().Be(200);

            var response = okResult.Value as ApiResponse<LoginResponse>;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data!.AccessToken.Should().Be("new-access-token");
        }

        [Fact]
        public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "invalid-refresh-token"
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenRequest.RefreshToken))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid refresh token"));

            // Act
            var result = await _controller.RefreshToken(refreshTokenRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            unauthorizedResult.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task RefreshToken_WithExpiredToken_ReturnsUnauthorized()
        {
            // Arrange
            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "expired-refresh-token"
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenRequest.RefreshToken))
                .ThrowsAsync(new UnauthorizedAccessException("Refresh token expired"));

            // Act
            var result = await _controller.RefreshToken(refreshTokenRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            unauthorizedResult.StatusCode.Should().Be(401);
            var response = unauthorizedResult.Value as ApiResponse;
            response!.Message.Should().Contain("expired");
        }

        #endregion

        #region ValidateToken Tests

        [Fact]
        public async Task ValidateToken_WithValidToken_ReturnsOkWithValidationResult()
        {
            // Arrange
            var validateTokenRequest = new ValidateTokenRequest
            {
                Token = "valid-token"
            };

            var validationResponse = new ValidateTokenResponse
            {
                IsValid = true,
                UserId = Guid.NewGuid().ToString(),
                Email = "test@example.com",
                Roles = new List<string> { "Admin" }
            };

            _mockAuthService.Setup(x => x.ValidateTokenAsync(validateTokenRequest.Token))
                .ReturnsAsync(validationResponse);

            // Act
            var result = await _controller.ValidateToken(validateTokenRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.StatusCode.Should().Be(200);

            var response = okResult.Value as ApiResponse<ValidateTokenResponse>;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data!.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateToken_WithInvalidToken_ReturnsOkWithInvalidFlag()
        {
            // Arrange
            var validateTokenRequest = new ValidateTokenRequest
            {
                Token = "invalid-token"
            };

            var validationResponse = new ValidateTokenResponse
            {
                IsValid = false
            };

            _mockAuthService.Setup(x => x.ValidateTokenAsync(validateTokenRequest.Token))
                .ReturnsAsync(validationResponse);

            // Act
            var result = await _controller.ValidateToken(validateTokenRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as ApiResponse<ValidateTokenResponse>;
            response!.Data!.IsValid.Should().BeFalse();
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task Logout_WithValidToken_ReturnsOk()
        {
            // Arrange
            var logoutRequest = new LogoutRequest
            {
                RefreshToken = "valid-refresh-token"
            };

            _mockAuthService.Setup(x => x.RevokeRefreshTokenAsync(logoutRequest.RefreshToken))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout(logoutRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.StatusCode.Should().Be(200);

            var response = okResult.Value as ApiResponse;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Message.Should().Contain("logged out");
        }

        #endregion
    }
}
