using Microsoft.IdentityModel.Tokens;
using Piolax_WebApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Piolax_WebApp.Services.Impl
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
            var tokenKey = config["TokenKey"];
            if (string.IsNullOrEmpty(tokenKey))
            {
                throw new ArgumentNullException(nameof(tokenKey), "TokenKey cannot be null or empty.");
            }
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        }

        public string CrearToken(Empleado empleado)
        {
            var claims = new[]
            {
                   new Claim(JwtRegisteredClaimNames.Sub, empleado.numNomina),
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var notBefore = DateTime.UtcNow;
            var expires = notBefore.AddMinutes(_config.GetValue<int>("TokenExpirationMinutes")); // Configurar el tiempo de expiración

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                notBefore: notBefore,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ObtenerClaimsPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    if (jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return principal;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
