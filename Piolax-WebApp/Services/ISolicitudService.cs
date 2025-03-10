using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface ISolicitudService
    {
        Task<SolicitudesDetalleDTO> RegistrarSolicitud(SolicitudesDTO solicitudesDTO);
        Task<SolicitudesDetalleDTO> ObtenerSolicitudConDetalles(int idSolicitud);
        Task<IEnumerable<SolicitudesDetalleDTO>> ObtenerSolicitudes();
        Task<IEnumerable<SolicitudesDetalleDTO>> ObtenerSolicitudesEmpleado(string numNomina);
        Task<SolicitudesDetalleDTO> ModificarEstatusAprobacionSolicitante(int idSolicitud, int idStatusAprobacionSolicitante);
        Task<IEnumerable<Solicitudes>> ConsultarSolicitudesNoTomadas();
        Task<IEnumerable<SolicitudesDetalleDTO>> ConsultarSolicitudesTerminadas();
        Task ActualizarStatusOrden(int idSolicitud, int idStatusOrden);
        Task<IEnumerable<SolicitudesDetalleDTO>> ObtenerSolicitudesConPrioridadAsync();
        Task<bool> EliminarSolicitud(int idSolicitud);


    }
}
