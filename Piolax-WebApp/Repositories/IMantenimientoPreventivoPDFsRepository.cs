using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IMantenimientoPreventivoPDFsRepository
    {
        Task<IEnumerable<MantenimientoPreventivoPDFs>> ObtenerTodosLosMantenimientoPreventivoPDFs();
        Task<MantenimientoPreventivoPDFs> ObtenerMantenimientoPreventivoPDFsPorID(int idMPPDF);
        Task<MantenimientoPreventivoPDFs> AgregarMantenimientoPreventivoPDFs(MantenimientoPreventivoPDFs mantenimientoPreventivoPDFs);
        Task<MantenimientoPreventivoPDFs> EliminarMantenimientoPreventivoPDFs(int idMPPDF);
        Task<MantenimientoPreventivoPDFs> ConsultarPorNombrePDF (string nombrePDF);
    }
}
 