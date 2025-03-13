using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class MantenimientoPreventivoPDFsService (IMantenimientoPreventivoPDFsRepository repository): IMantenimientoPreventivoPDFsService
    {
        private readonly IMantenimientoPreventivoPDFsRepository _repository = repository;

        public async Task<MantenimientoPreventivoPDFsDTO> AgregarMantenimientoPreventivoPDFs(MantenimientoPreventivoPDFCrearDTO mantenimientoPreventivoCrearDTO)
        {
            var mantenimientoPreventivoPDFs = new MantenimientoPreventivoPDFs
            {
                idMP = mantenimientoPreventivoCrearDTO.idMP,
                nombrePDF = null,
                rutaPDF = mantenimientoPreventivoCrearDTO.rutaPDF
            };
            var mantenimientoPreventivoPDFsAgregado = await _repository.AgregarMantenimientoPreventivoPDFs(mantenimientoPreventivoPDFs);
            return new MantenimientoPreventivoPDFsDTO
            {
                idMP = mantenimientoPreventivoPDFsAgregado.idMP,
                rutaPDF = mantenimientoPreventivoPDFsAgregado.rutaPDF,
                nombrePDF = ExtraerNombrePDF(mantenimientoPreventivoPDFsAgregado.rutaPDF)
            };
        }

        // Mêtodo para extraer nombre de la URL 

        private string ExtraerNombrePDF(string rutaPDF)
        {
            var nombrePDF = rutaPDF.Split("/").Last();
            return nombrePDF;
        }
    }
}
