using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IMantenimientoPreventivoRepository
    {
       Task<MantenimientoPreventivo> CrearMantenimientoPreventico(MantenimientoPreventivo mantenimientoPreventivo);
       Task<MantenimientoPreventivo> ConsultarMP(int idMP);
       Task<MantenimientoPreventivo> Modificar(int idMP, MantenimientoPreventivo mantenimientoPreventivo);
       Task<MantenimientoPreventivo> Eliminar(int idMP);
    }
}
