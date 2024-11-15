using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface ISolicitudService
    {
        Task<Solicitudes> Consultar(int idSolicitud);
        Task<IEnumerable<Solicitudes>> ConsultarTodos();
        Task<Solicitudes> Registro(SolicitudesDTO solicitud);
        Task<Solicitudes> Modificar(int idSolicitud, SolicitudesDTO solicitud);
        Task<Solicitudes> Eliminar(int idSolicitud);
        Task<bool> SolicitudExiste(int idSolicitud);
    }
}
