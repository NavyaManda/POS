using System;
using System.Collections.Generic;

namespace AuthService.Models
{
    /// <summary>
    /// User entity for POSAuthDb
    /// </summary>
    public class User
    {
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    /// <summary>
    /// Refresh token for token rotation
    /// </summary>
    public class RefreshToken
    {
        public string TokenId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Login request DTO
    /// </summary>
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// Login response with tokens
    /// </summary>
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; } = 900; // 15 minutes
        public UserInfo User { get; set; }
    }

    /// <summary>
    /// User information in token response
    /// </summary>
    public class UserInfo
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; }
    }

    /// <summary>
    /// Token validation request
    /// </summary>
    public class ValidateTokenRequest
    {
        public string Token { get; set; }
    }

    /// <summary>
    /// Token validation response
    /// </summary>
    public class ValidateTokenResponse
    {
        public bool IsValid { get; set; }
        public string UserId { get; set; }
        public List<string> Roles { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Refresh token request
    /// </summary>
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// Registration request DTO
    /// </summary>
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
