using System.Security.Claims;

namespace AuthService.API.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userId, string email, List<string> roles);
        string GenerateRefreshToken();
        TokenClaims ValidateToken(string token);
    }

    public class TokenClaims
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public long IssuedAt { get; set; }
        public long ExpiresAt { get; set; }
    }
}
