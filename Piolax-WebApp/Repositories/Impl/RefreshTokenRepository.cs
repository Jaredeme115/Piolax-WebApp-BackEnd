using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;
using System.Globalization;
using System.Security.Cryptography;

namespace Piolax_WebApp.Repositories.Impl
{
    public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<RefreshTokens> AddAsync(RefreshTokens refreshTokens)
        {
            try
            {
                _context.RefreshTokens.Add(refreshTokens);
                await _context.SaveChangesAsync();
                return refreshTokens;
            }
            catch (Exception ex)
            {
                // Registra el error para depuración
                throw new Exception("Error al agregar el refresh token.", ex);
            }
        }


        public async Task<RefreshTokens> GetByTokenAsync(string token)
        {
            try
            {
                return await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.token == token && !rt.revoked);
            }
            catch (Exception ex)
            {
                // Log de errores
                throw new Exception("Error al buscar el refresh token.", ex);
            }
        }

        public async Task<bool> RevokeAsync(string token)
        {
            var refreshTokens = await GetByTokenAsync(token);
            if (refreshTokens == null) return false;

            refreshTokens.revoked = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
