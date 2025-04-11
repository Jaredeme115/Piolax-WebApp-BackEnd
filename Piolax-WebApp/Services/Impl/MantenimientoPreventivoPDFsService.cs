using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class MantenimientoPreventivoPDFsService (IMantenimientoPreventivoPDFsRepository repository): IMantenimientoPreventivoPDFsService
    {
        private readonly IMantenimientoPreventivoPDFsRepository _repository = repository;

        private readonly string _pdfBaseUrl = "https://localhost:7208"; // Ajusta si tu backend está en otro puerto



        public async Task<MantenimientoPreventivoPDFs> AgregarMantenimientoPreventivoPDFs(MantenimientoPreventivoPDFCrearDTO mantenimientoPreventivoPDFCrearDTO)
        {
            if (string.IsNullOrWhiteSpace(mantenimientoPreventivoPDFCrearDTO.rutaPDF))
            {
                throw new ArgumentException("La ruta del PDF no puede estar vacía.");
            }

            if (mantenimientoPreventivoPDFCrearDTO.idMP <= 0)
            {
                throw new ArgumentException("El ID del mantenimiento preventivo no es válido.");
            }

            var mantenimientoPreventivoPDFs = new MantenimientoPreventivoPDFs
            {
                idMP = mantenimientoPreventivoPDFCrearDTO.idMP,
                nombrePDF = ExtraerNombrePDF(mantenimientoPreventivoPDFCrearDTO.rutaPDF),
                rutaPDF = mantenimientoPreventivoPDFCrearDTO.rutaPDF
            };

            return await _repository.AgregarMantenimientoPreventivoPDFs(mantenimientoPreventivoPDFs);
        }

        public async Task<IEnumerable<MantenimientoPreventivoPDFsDTO>> ObtenerTodosLosMantenimientoPreventivoPDFs()
        {
            var resultados = await _repository.ObtenerTodosLosMantenimientoPreventivoPDFs();
            return resultados.Select(mppdf => new MantenimientoPreventivoPDFsDTO
            {
                idMP = mppdf.idMP,
                nombrePDF = mppdf.nombrePDF,
                //rutaPDF = mppdf.rutaPDF
                rutaPDF = $"{_pdfBaseUrl}{mppdf.rutaPDF}"
            });
        }

        public async Task<MantenimientoPreventivoPDFsDTO?> ObtenerMantenimientoPreventivoPDFsPorID(int idMPPDF)
        {
            var resultado = await _repository.ObtenerMantenimientoPreventivoPDFsPorID(idMPPDF);
            if (resultado == null)
                return null;

            return new MantenimientoPreventivoPDFsDTO
            {
                idMP = resultado.idMP,
                nombrePDF = resultado.nombrePDF,
                //rutaPDF = resultado.rutaPDF
                rutaPDF = $"{_pdfBaseUrl}{resultado.rutaPDF}"
            };
        }

        public async Task<MantenimientoPreventivoPDFsDTO> EliminarMantenimientoPreventivoPDFs(int idMPPDF)
        {
            var resultado = await _repository.EliminarMantenimientoPreventivoPDFs(idMPPDF);
            if (resultado == null)
                return null;

            return new MantenimientoPreventivoPDFsDTO
            {
                idMP = resultado.idMP,
                nombrePDF = resultado.nombrePDF,
                //rutaPDF = resultado.rutaPDF
                rutaPDF = $"{_pdfBaseUrl}{resultado.rutaPDF}"
            };
        }

        public async Task<MantenimientoPreventivoPDFsDTO> ConsultarPorNombrePDF(string nombrePDF)
        {
            var resultado = await _repository.ConsultarPorNombrePDF(nombrePDF);
            if (resultado == null)
                return null;

            return new MantenimientoPreventivoPDFsDTO
            {
                idMP = resultado.idMP,
                nombrePDF = resultado.nombrePDF,
                //rutaPDF = resultado.rutaPDF
                rutaPDF = $"{_pdfBaseUrl}{resultado.rutaPDF}"
            };
        }

        public async Task<IEnumerable<MantenimientoPreventivoPDFsDTO>> ObtenerPDFsPorMantenimientoPreventivo(int idMP)
        {
            var resultados = await _repository.ObtenerPDFsPorMantenimientoPreventivo(idMP);

            return resultados.Select(mppdf => new MantenimientoPreventivoPDFsDTO
            {
                idMP = mppdf.idMP,
                nombrePDF = mppdf.nombrePDF,
                rutaPDF = $"{_pdfBaseUrl}{mppdf.rutaPDF}" // 👈 Devuelve la URL completa
            });
        }



        // Método mejorado para extraer el nombre del PDF de la URL
        private string ExtraerNombrePDF(string rutaPDF)
        {
            return rutaPDF.Split('/').LastOrDefault() ?? "archivo_desconocido.pdf"; // Evita errores si la ruta no tiene "/"
        }

    }
}
