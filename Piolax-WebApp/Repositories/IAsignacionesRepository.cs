using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IAsignacionesRepository
    {
        Task<Asignaciones> RegistrarAsignacion(Asignaciones asignaciones);
        Task<Asignaciones> ObtenerAsignacionConDetalles(int idAsignacion);
        //Task<IEnumerable<Asignaciones>> ObtenerTodasLasAsignaciones();
        //Task<IEnumerable<Asignaciones>> ObtenerAsignacionesPorTecnico(string numNomina);

        //Método para obtener modificar el estatus de aprobación de la asignacion
        //Task<SolicitudesDetalleDTO> ModificarEstatusAprobacionTecnico(int idAsignacion, int idStatusAprobacionTecnico);
    }
}
