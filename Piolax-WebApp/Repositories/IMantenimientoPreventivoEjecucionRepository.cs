using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IMantenimientoPreventivoEjecucionRepository
    {
        Task<MantenimientoPreventivoEjecuciones> CrearAsync(MantenimientoPreventivoEjecuciones ejecucion);

        Task<IEnumerable<MantenimientoPreventivoEjecuciones>> ObtenerPorMPAsync(int idMP);


    }
}
