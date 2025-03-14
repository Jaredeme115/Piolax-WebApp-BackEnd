using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IMantenimientoPreventivoPDFsService
    {
        //Task<IEnumerable<MantenimientoPreventivoPDFsDTO>> ObtenerTodosLosMantenimientoPreventivoPDFs();
        //Task<MantenimientoPreventivoPDFsDTO> ObtenerMantenimientoPreventivoPDFsPorID(int idMPPDF);
        Task<MantenimientoPreventivoPDFs> AgregarMantenimientoPreventivoPDFs(MantenimientoPreventivoPDFsDTO mantenimientoPreventivoPDFsDTO);
        //Task<MantenimientoPreventivoPDFs> EliminarMantenimientoPreventivoPDFs(int idMPPDF);
        //Task<MantenimientoPreventivoPDFsDTO> ConsultarPorNombrePDF(string nombrePDF);
    }
}
