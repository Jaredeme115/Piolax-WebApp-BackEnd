using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokens> AddAsync(RefreshTokens refreshTokens);
        Task<RefreshTokens> GetByTokenAsync(string token);
        Task<bool> RevokeAsync(string token);
    }
}
