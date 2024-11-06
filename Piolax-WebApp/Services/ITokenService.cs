using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface ITokenService
    {
       string CrearToken(Empleado empleado);
    }
}
