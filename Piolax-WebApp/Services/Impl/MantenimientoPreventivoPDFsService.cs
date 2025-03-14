using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class MantenimientoPreventivoPDFsService (IMantenimientoPreventivoPDFsRepository repository): IMantenimientoPreventivoPDFsService
    {
        private readonly IMantenimientoPreventivoPDFsRepository _repository = repository;


        public async Task<MantenimientoPreventivoPDFs> AgregarMantenimientoPreventivoPDFs(MantenimientoPreventivoPDFsDTO mantenimientoPreventivoPDFsDTO)
        {
            // Validar si la rutaPDF es nula o vacía
            if (string.IsNullOrWhiteSpace(mantenimientoPreventivoPDFsDTO.rutaPDF))
            {
                throw new ArgumentException("La ruta del PDF no puede estar vacía.");
            }

            // Validar si el idMP es válido
            if (mantenimientoPreventivoPDFsDTO.idMP <= 0)
            {
                throw new ArgumentException("El ID del mantenimiento preventivo no es válido.");
            }

            var mantenimientoPreventivoPDFs = new MantenimientoPreventivoPDFs
            {
                idMP = mantenimientoPreventivoPDFsDTO.idMP,
                nombrePDF = ExtraerNombrePDF(mantenimientoPreventivoPDFsDTO.rutaPDF),
                rutaPDF = mantenimientoPreventivoPDFsDTO.rutaPDF
            };

            return await _repository.AgregarMantenimientoPreventivoPDFs(mantenimientoPreventivoPDFs);
        }

        // Método mejorado para extraer el nombre del PDF de la URL
        private string ExtraerNombrePDF(string rutaPDF)
        {
            return rutaPDF.Split('/').LastOrDefault() ?? "archivo_desconocido.pdf"; // Evita errores si la ruta no tiene "/"
        }

    }
}
