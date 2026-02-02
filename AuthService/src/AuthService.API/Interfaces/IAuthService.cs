using AuthService.API.Models;

namespace AuthService.API.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(string refreshToken);
        Task RegisterAsync(RegisterRequest request);
        Task<ValidateTokenResponse> ValidateTokenAsync(string token);
    }
}
