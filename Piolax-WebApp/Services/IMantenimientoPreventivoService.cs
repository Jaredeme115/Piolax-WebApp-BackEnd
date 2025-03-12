using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IMantenimientoPreventivoService
    {
        Task<MantenimientoPreventivoDTO> CrearMantenimientoPreventivo(MantenimientoPreventivoCreateDTO mantenimientoPreventivoDTO);
        Task<MantenimientoPreventivoDetallesDTO> ConsultarMPConDetalles(int idMP);
        Task<MantenimientoPreventivoDTO> ModificarMantenimientoPreventivo(int idMP, MantenimientoPreventivoModificarDTO mantenimientoPreventivoModificarDTO);
        Task<bool> EliminarMantenimientoPreventivo(int idMP);
    }
}
