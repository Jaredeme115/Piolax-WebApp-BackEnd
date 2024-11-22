using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface ISolicitudesRepository
    {
        Task<Solicitudes> RegistrarSolicitud(Solicitudes solicitudes);
        Task<Solicitudes> ObtenerSolicitudConDetalles(int idSolicitud);
        Task<IEnumerable<Solicitudes>> ObtenerSolicitudes();

    }
}
