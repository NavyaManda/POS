using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Xunit;
using Moq;
using FluentAssertions;
using AuthService.API.Services;
using AuthService.API.Models;
using Microsoft.Extensions.Configuration;

namespace AuthService.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly TokenService _tokenService;
        private const string ValidSecret = "this-is-a-very-long-secret-key-that-is-more-than-32-characters";
        private const string ValidIssuer = "pos-system";
        private const string ValidAudience = "pos-api";
        private const string ValidExpiryMinutes = "15";

        public TokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            SetupDefaultConfiguration();
            _tokenService = new TokenService(_mockConfiguration.Object);
        }

        private void SetupDefaultConfiguration()
        {
            _mockConfiguration.Setup(x => x["Jwt:Secret"]).Returns(ValidSecret);
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns(ValidIssuer);
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns(ValidAudience);
            _mockConfiguration.Setup(x => x["Jwt:ExpiryMinutes"]).Returns(ValidExpiryMinutes);
        }

        #region GenerateAccessToken Tests

        [Fact]
        public void GenerateAccessToken_WithValidParameters_ReturnsValidToken()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var email = "test@example.com";
            var roles = new List<string> { "Admin", "User" };

            // Act
            var token = _tokenService.GenerateAccessToken(userId, email, roles);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Should().Match("eyJ*");
        }

        [Fact]
        public void GenerateAccessToken_TokenCanBeParsed_ContainsCorrectClaims()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var email = "test@example.com";
            var roles = new List<string> { "Admin" };

            // Act
            var token = _tokenService.GenerateAccessToken(userId, email, roles);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            // Assert
            jwtToken.Should().NotBeNull();
            jwtToken!.Claims.Should().Contain(c => c.Type == "sub" && c.Value == userId);
            jwtToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == email);
            jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Admin");
        }

        [Fact]
        public void GenerateAccessToken_TokenExpiry_IsSetCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var email = "test@example.com";
            var roles = new List<string>();

            // Act
            var beforeGeneration = DateTime.UtcNow;
            var token = _tokenService.GenerateAccessToken(userId, email, roles);
            var afterGeneration = DateTime.UtcNow.AddMinutes(15);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            // Assert
            jwtToken?.ValidTo.Should().BeBetween(beforeGeneration.AddMinutes(15), afterGeneration.AddSeconds(1));
        }

        [Fact]
        public void GenerateAccessToken_WithMultipleRoles_IncludesAllRoles()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var email = "test@example.com";
            var roles = new List<string> { "Admin", "Manager", "User" };

            // Act
            var token = _tokenService.GenerateAccessToken(userId, email, roles);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            // Assert
            var roleClaims = jwtToken!.Claims.Where(c => c.Type == "role").ToList();
            roleClaims.Should().HaveCount(3);
            roleClaims.Should().Contain(c => c.Value == "Admin");
            roleClaims.Should().Contain(c => c.Value == "Manager");
            roleClaims.Should().Contain(c => c.Value == "User");
        }

        [Fact]
        public void GenerateAccessToken_WithoutSecret_ThrowsInvalidOperationException()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["Jwt:Secret"]).Returns((string)null);
            var tokenService = new TokenService(configuration.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                tokenService.GenerateAccessToken(Guid.NewGuid().ToString(), "test@example.com", new List<string>())
            );
        }

        #endregion

        #region GenerateRefreshToken Tests

        [Fact]
        public void GenerateRefreshToken_ReturnsValidBase64String()
        {
            // Act
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Assert
            refreshToken.Should().NotBeNullOrEmpty();
            // Should not throw if valid base64
            Assert.Throws<FormatException>(() => Convert.FromBase64String(refreshToken + "!")); // Invalid base64 should throw
            var bytes = Convert.FromBase64String(refreshToken); // Valid base64 should not throw
            bytes.Length.Should().Be(32); // Random number is 32 bytes
        }

        [Fact]
        public void GenerateRefreshToken_GenerateMultiple_AllUnique()
        {
            // Act
            var tokens = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                tokens.Add(_tokenService.GenerateRefreshToken());
            }

            // Assert
            tokens.Distinct().Should().HaveCount(10); // All unique
        }

        #endregion

        #region ValidateToken Tests

        [Fact]
        public void ValidateToken_WithValidToken_ReturnsTokenClaims()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var email = "test@example.com";
            var roles = new List<string> { "Admin", "User" };
            var token = _tokenService.GenerateAccessToken(userId, email, roles);

            // Act
            var claims = _tokenService.ValidateToken(token);

            // Assert
            claims.Should().NotBeNull();
            claims.UserId.Should().Be(userId);
            claims.Email.Should().Be(email);
            claims.Roles.Should().Contain("Admin");
            claims.Roles.Should().Contain("User");
        }

        [Fact]
        public void ValidateToken_WithInvalidToken_ThrowsSecurityTokenException()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _tokenService.ValidateToken(invalidToken));
        }

        [Fact]
        public void ValidateToken_WithTamperedToken_ThrowsSecurityTokenSignatureKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var email = "test@example.com";
            var roles = new List<string>();
            var token = _tokenService.GenerateAccessToken(userId, email, roles);
            var tamperedToken = token.Substring(0, token.Length - 5) + "XXXXX"; // Tamper with signature

            // Act & Assert
            Assert.Throws<Exception>(() => _tokenService.ValidateToken(tamperedToken));
        }

        [Fact]
        public void ValidateToken_WithExpiredToken_ThrowsSecurityTokenExpiredException()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["Jwt:Secret"]).Returns(ValidSecret);
            configuration.Setup(x => x["Jwt:Issuer"]).Returns(ValidIssuer);
            configuration.Setup(x => x["Jwt:Audience"]).Returns(ValidAudience);
            configuration.Setup(x => x["Jwt:ExpiryMinutes"]).Returns("-1"); // Already expired
            var tokenService = new TokenService(configuration.Object);

            var userId = Guid.NewGuid().ToString();
            var email = "test@example.com";
            var token = tokenService.GenerateAccessToken(userId, email, new List<string>());

            // Wait a bit to ensure token is expired
            System.Threading.Thread.Sleep(1100);

            // Act & Assert
            Assert.Throws<Exception>(() => _tokenService.ValidateToken(token));
        }

        #endregion
    }
}
