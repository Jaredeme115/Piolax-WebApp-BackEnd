using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface ISolicitudesRepository
    {
        Task<Solicitudes> Consultar(int idSolicitud);
        Task<IEnumerable<Solicitudes>> ConsultarTodos();
        Task<Solicitudes> Registro(Solicitudes solicitudes);
        Task<Solicitudes> Modificar(int idSolicitud, Solicitudes solicitudes);
        Task<Solicitudes> Eliminar(int idSolicitud);
        Task<bool> SolicitudExiste(int idSolicitud);
    }
}
