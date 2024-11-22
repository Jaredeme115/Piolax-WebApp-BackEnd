using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IRefreshTokensService
    {
        Task<RefreshTokens> GenerateRefreshToken(int idEmpleado);
        Task<RefreshTokens> GetRefreshToken(string token);
        Task RevokeRefreshToken(string token);
       
    }
}
