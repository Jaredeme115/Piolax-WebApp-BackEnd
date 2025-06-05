using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using System.Globalization;
using System.Threading.Tasks;

namespace Piolax_WebApp.Repositories
{
    public interface ISolicitudesRepository
    {
        Task<Solicitudes> RegistrarSolicitud(Solicitudes solicitudes);
        Task<Solicitudes> ObtenerSolicitudConDetalles(int idSolicitud);
        Task<IEnumerable<Solicitudes>> ObtenerSolicitudes();
        Task<IEnumerable<Solicitudes>> ObtenerSolicitudesEmpleado(string numNomina);
        Task<bool> ExisteSolicitud(int idSolicitud);

        //Método para obtener modificar el estatus de aprobación de la solicitud
        Task<SolicitudesDetalleDTO> ModificarEstatusAprobacionSolicitante(int idSolicitud, int idStatusAprobacionSolicitante);
        Task<IEnumerable<Solicitudes>> ConsultarSolicitudesPorMaquinaYArea(int idMaquina, int idArea);
        Task<IEnumerable<Solicitudes>> ConsultarSolicitudesNoTomadas();
        Task<IEnumerable<Solicitudes>> ConsultarSolicitudesTerminadas();
        Task ActualizarStatusOrden(int idSolicitud, int idStatusOrden);
        Task<IEnumerable<Solicitudes>> ObtenerSolicitudesConPrioridadAsync();
        Task<bool> EliminarSolicitud(int idSolicitud);

        //Método para obtener solicitudes por area
        Task<IEnumerable<Solicitudes>> ObtenerSolicitudesPorAreaYRoles(int idArea, List<int> idRoles);
        Task<List<Solicitudes>> ObtenerSolicitudesEnStatus(int idStatusOrden);

        //Método de apoyo para calculo de MTBF
        Task<int> ContarFallasPorAreaEnMes(int idArea, int anio, int mes);

    }
}
