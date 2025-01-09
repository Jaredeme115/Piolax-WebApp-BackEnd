using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IAsignacionesRepository
    {
        Task<Asignaciones> RegistrarAsignacion(Asignaciones asignaciones);
        Task<Asignaciones> ObtenerAsignacioConDetalles(int idAsignacion);
        Task<IEnumerable<Asignaciones>> ObtenerAsignaciones();
        Task<IEnumerable<Asignaciones>> ObtenerAsignacionTecnico(string numNomina);

        //Método para obtener modificar el estatus de aprobación de la asignacion
        Task<AsignacionesDetalleDTO> ModificarEstatusAprobacionTecnico(int idAsignacion, int idStatusAprobacionTecnico);
    }
}
