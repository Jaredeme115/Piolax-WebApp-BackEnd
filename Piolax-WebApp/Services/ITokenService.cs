using Piolax_WebApp.Models;
using System.Security.Claims;

namespace Piolax_WebApp.Services
{
    public interface ITokenService
    {
       string CrearToken(Empleado empleado);
       ClaimsPrincipal ObtenerClaimsPrincipal(string token);

    }
}
