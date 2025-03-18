using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IMantenimientoPreventivoRefaccionesService
    {
        Task<IEnumerable<MantenimientoPreventivo_Refacciones>> ConsultarRefaccionesMP(int idMP);
        Task<MantenimientoPreventivoRefaccionesDTO> CrearRefaccionMP(MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones);
        Task<IEnumerable<MantenimientoPreventivo_Refacciones>> ConsultarTodasLasRefacciones();
        Task<bool> EliminarRefaccionMP(int idMPRefaccion);
        Task<bool> ActualizarRefaccionMP(MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones);
        Task<MantenimientoPreventivoRefaccionesDetalleDTO> ConsultarRefaccionPorId(int idMPRefaccion);
    }
}
