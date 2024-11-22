using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<RefreshTokens> AddAsync(RefreshTokens refreshTokens)
        {
            _context.RefreshTokens.Add(refreshTokens);
            await _context.SaveChangesAsync();
            return refreshTokens;
        }

        public async Task<RefreshTokens?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.token == token && !rt.revoked);
        }

        public async Task RevokeAsync(string token)
        {
            var refreshTokens = await GetByTokenAsync(token);
            if (refreshTokens != null)
            {
                refreshTokens.revoked = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
