using Microsoft.IdentityModel.Tokens;
using Piolax_WebApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Piolax_WebApp.Services.Impl
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            var tokenKey = config["TokenKey"];
            if (string.IsNullOrEmpty(tokenKey))
            {
                throw new ArgumentNullException(nameof(tokenKey), "TokenKey cannot be null or empty.");
            }
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        }

        public string CrearToken(Empleado empleado)
        {
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.NameId, empleado.numNomina.ToString())
                };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
