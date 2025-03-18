using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IMantenimientoPreventivoRefaccionesRepository
    {
        Task<IEnumerable<MantenimientoPreventivo_Refacciones>> ConsultarRefaccionesMP(int idMP);
        Task<MantenimientoPreventivo_Refacciones> CrearRefaccionMP(MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones);
        Task<IEnumerable<MantenimientoPreventivo_Refacciones>> ConsultarTodasLasRefacciones();
        Task<bool> EliminarRefaccionMP(int idMPRefaccion);
        Task<bool> ActualizarRefaccionMP(MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones);
        Task<MantenimientoPreventivo_Refacciones> ConsultarRefaccionPorId(int idMPRefaccion);
    }
}
