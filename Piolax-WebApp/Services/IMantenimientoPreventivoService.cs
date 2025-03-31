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
        Task<bool> MarcarComoRealizado(int idMP);
        Task<IEnumerable<MantenimientoPreventivoDetallesDTO>> ConsultarTodosMPsDTO();
        Task<IEnumerable<MantenimientoPreventivoDetallesDTO>> MostrarMPsAsignados(int idEmpleado);
        Task<MantenimientoPreventivoDetallesDTO> ActivarMantenimientoPreventivo(int idMP);
        Task<MantenimientoPreventivoDetallesDTO> DesactivarMantenimientoPreventivo(int idMP);
        Task<MantenimientoPreventivoDetallesDTO> CambiarEstatusEnProceso(int idMP);
        Task<MantenimientoPreventivoDetallesDTO> CancelarMantenimientoEnProceso(int idMP);
    }
}
