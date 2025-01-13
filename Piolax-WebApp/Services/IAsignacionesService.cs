using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IAsignacionesService
    {
        Task<AsignacionesDetalleDTO> RegistrarAsignacion(AsignacionesDTO asignacionesDTO);
        Task<AsignacionesDetalleDTO> ObtenerAsignacionConDetalles(int idAsignacion);
        Task<IEnumerable<AsignacionesDetalleDTO>> ObtenerTodasLasAsignaciones();
        Task<IEnumerable<AsignacionesDetalleDTO>> ObtenerAsignacionPorTecnico(string numNomina);
        Task<AsignacionesDetalleDTO> ModificarEstatusAprobacionTecnico(int idAsignacion, int idStatusAprobacionTecnico);
    }
}
