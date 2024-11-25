using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using System.Security.Cryptography;

namespace Piolax_WebApp.Services.Impl
{
    public class RefreshTokensService(IRefreshTokenRepository repository) : IRefreshTokensService
    {
        private readonly IRefreshTokenRepository _repository = repository;

        public async Task<RefreshTokens> GenerateRefreshToken(int idEmpleado)
        {
            var refreshToken = new RefreshTokens
            {
                token = GenerateBase64UrlToken(64), // 64 bytes generan un token suficientemente seguro
                idEmpleado = idEmpleado,
                expiresAt = DateTime.Now.AddDays(7) // El refresh token expira en 7 días
            };

            return await _repository.AddAsync(refreshToken);
        }

        public async Task<RefreshTokens> GetRefreshToken(string token)
        {
            return await _repository.GetByTokenAsync(token);
        }

        public async Task RevokeRefreshToken(string token)
        {
            await _repository.RevokeAsync(token);
        }

        private string GenerateBase64UrlToken(int byteLength)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(byteLength);
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-") // Sustituye '+' por '-'
                .Replace("/", "_") // Sustituye '/' por '_'
                .Replace("=", ""); // Elimina '=' (relleno)
        }

    }
}
