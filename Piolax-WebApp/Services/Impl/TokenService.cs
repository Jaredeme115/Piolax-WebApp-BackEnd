using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
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

        //Agregago para pruebas con la politica de roles
        private readonly AppDbContext _context;

        public TokenService(IConfiguration config, AppDbContext context)
        {
            _config = config;
            var tokenKey = config["TokenKey"];
            if (string.IsNullOrEmpty(tokenKey))
            {
                throw new ArgumentNullException(nameof(tokenKey), "TokenKey cannot be null or empty.");
            }
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            _context = context;
        }

        public string CrearToken(Empleado empleado)
        {
            var claims = new List<Claim>
    {
           new Claim(ClaimTypes.NameIdentifier, empleado.idEmpleado.ToString()),
           new Claim(JwtRegisteredClaimNames.Sub, empleado.numNomina),
           new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            // Obtener los roles y áreas del empleado desde la base de datos
            var empleadoAreaRoles = _context.EmpleadoAreaRol
                .Where(ear => ear.idEmpleado == empleado.idEmpleado)
                .Include(ear => ear.Rol)
                .ToList();

            // Usamos una variable para almacenar las combinaciones área-rol para debugging
            var combinaciones = new List<string>();

            // Agregar roles a los claims y guardar los idRol
            foreach (var ear in empleadoAreaRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, ear.Rol.nombreRol));
                claims.Add(new Claim("idRol", ear.idRol.ToString()));  // Este es el cambio clave
                claims.Add(new Claim("idArea", ear.idArea.ToString()));

                // Guardar combinación para debugging
                combinaciones.Add($"Area: {ear.idArea}, Rol: {ear.idRol}");
            }

            // Para debugging - puedes comentar o quitar esto en producción
            Console.WriteLine($"Token generado para empleado {empleado.idEmpleado} con combinaciones: {string.Join("; ", combinaciones)}");

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var notBefore = DateTime.UtcNow;
            var expires = notBefore.AddMinutes(_config.GetValue<int>("TokenExpirationMinutes"));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                notBefore: notBefore,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
