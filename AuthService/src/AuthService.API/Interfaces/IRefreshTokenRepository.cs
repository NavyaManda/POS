using AuthService.API.Models;

namespace AuthService.API.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken?> GetByIdAsync(Guid id);
        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId);
        Task<RefreshToken> AddAsync(RefreshToken refreshToken);
        Task DeleteAsync(Guid id);
        Task<bool> IsValidAsync(string token);
        Task RevokeAllUserTokensAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
