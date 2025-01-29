using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IStatusAsignacionService
    {
        Task<StatusAsignacion> Consultar (int idStatusAsignacion);
    }
}
