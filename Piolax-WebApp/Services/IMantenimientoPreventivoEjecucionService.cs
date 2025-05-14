using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IMantenimientoPreventivoEjecucionService
    {
        Task<MantenimientoPreventivoEjecucionDTO> CrearEjecucionAsync(MantenimientoPreventivoEjecucionCrearDTO mantenimientoPreventivoEjecucionCrearDTO);
        Task<IEnumerable<MantenimientoPreventivoEjecucionDTO>> ObtenerEjecucionesPorMPAsync(int idMP);
    }
}
