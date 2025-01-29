using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IStatusAsignacionRepository
    {
        Task<StatusAsignacion> Consultar(int idStatusAsignacion);
    }
}
