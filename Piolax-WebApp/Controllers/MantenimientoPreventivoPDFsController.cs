using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class MantenimientoPreventivoPDFsController(IMantenimientoPreventivoPDFsService service): BaseApiController
    {
        private readonly IMantenimientoPreventivoPDFsService _service = service;

        [HttpPost("AgregarMantenimientoPreventivoPDFs")]
        public async Task<ActionResult<MantenimientoPreventivoPDFsDTO>> AgregarMantenimientoPreventivoPDFs(MantenimientoPreventivoPDFCrearDTO mantenimientoPreventivoPDFCrearDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _service.AgregarMantenimientoPreventivoPDFs(mantenimientoPreventivoPDFCrearDTO);

                if (resultado == null)
                {
                    return StatusCode(500, "Error al guardar el PDF en la base de datos.");
                }

                return Ok(new MantenimientoPreventivoPDFsDTO
                {
                    idMP = resultado.idMP,
                    nombrePDF = resultado.nombrePDF,
                    rutaPDF = resultado.rutaPDF
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("ObtenerTodosLosMantenimientoPreventivoPDFs")]
        public async Task<ActionResult<IEnumerable<MantenimientoPreventivoPDFsDTO>>> ObtenerTodosLosMantenimientoPreventivoPDFs()
        {
            var resultado = await _service.ObtenerTodosLosMantenimientoPreventivoPDFs();
            return Ok(resultado);
        }

        [HttpGet("ObtenerMantenimientoPreventivoPDFsPorID/{idMPPDF}")]
        public async Task<ActionResult<MantenimientoPreventivoPDFsDTO>> ObtenerMantenimientoPreventivoPDFsPorID(int idMPPDF)
        {
            var resultado = await _service.ObtenerMantenimientoPreventivoPDFsPorID(idMPPDF);
            if (resultado == null)
            {
                return NotFound($"No se encontró el PDF con ID {idMPPDF}.");
            }
            return Ok(resultado);
        }

        [HttpDelete("EliminarMantenimientoPreventivoPDFs/{idMPPDF}")]
        public async Task<ActionResult<MantenimientoPreventivoPDFsDTO>> EliminarMantenimientoPreventivoPDFs(int idMPPDF)
        {
            var resultado = await _service.EliminarMantenimientoPreventivoPDFs(idMPPDF);
            if (resultado == null)
            {
                return NotFound($"No se encontró el PDF con ID {idMPPDF}.");
            }
            return Ok(resultado);
        }

        [HttpGet("ConsultarPorNombrePDF/{nombrePDF}")]
        public async Task<ActionResult<MantenimientoPreventivoPDFsDTO>> ConsultarPorNombrePDF(string nombrePDF)
        {
            var resultado = await _service.ConsultarPorNombrePDF(nombrePDF);
            if (resultado == null)
            {
                return NotFound($"No se encontró el PDF con el nombre {nombrePDF}.");
            }
            return Ok(resultado);
        }

        [HttpGet("ObtenerPDFsPorMantenimientoPreventivo/{idMP}")]
        public async Task<ActionResult<IEnumerable<MantenimientoPreventivoPDFsDTO>>> ObtenerPDFsPorMantenimientoPreventivo(int idMP)
        {
            try
            {
                var resultados = await _service.ObtenerPDFsPorMantenimientoPreventivo(idMP);
                if (resultados == null || !resultados.Any())
                {
                    return NotFound("No se encontraron PDFs para este mantenimiento preventivo.");
                }
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("ListarPDFsPorArea/{idArea}")]
        public IActionResult ListarPDFsPorArea(int idArea)
        {
            var nombreCarpeta = idArea switch
            {
                1 => "Metales",
                2 => "Ensamble",
                3 => "Plasticos",
                19 => "Facility",
                _ => null
            };

            if (string.IsNullOrEmpty(nombreCarpeta))
                return BadRequest("Área no válida.");

            var rutaBase = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PDFs", nombreCarpeta);

            if (!Directory.Exists(rutaBase))
                return NotFound("No se encontró la carpeta del área.");

            var archivos = Directory.GetFiles(rutaBase, "*.pdf")
                                    .Select(path => new
                                    {
                                        nombrePDF = Path.GetFileName(path),
                                        rutaPDF = $"/PDFs/{nombreCarpeta}/{Path.GetFileName(path)}"
                                    });

            return Ok(archivos);
        }

        [HttpGet("BuscarPDFs")] 
        public ActionResult<IEnumerable<MantenimientoPreventivoPDFsDTO>> BuscarPDFs([FromQuery] string nombre)
        {
            var resultados = _service.BuscarPDFsPorNombre(nombre);
            return Ok(resultados);
        }
    }
}
