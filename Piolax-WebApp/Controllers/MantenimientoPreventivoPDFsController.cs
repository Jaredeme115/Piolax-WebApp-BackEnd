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
        public async Task<ActionResult<MantenimientoPreventivoPDFs>> AgregarMantenimientoPreventivoPDFs(MantenimientoPreventivoPDFsDTO mantenimientoPreventivoPDFsDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _service.AgregarMantenimientoPreventivoPDFs(mantenimientoPreventivoPDFsDTO);

                if (resultado == null)
                {
                    return StatusCode(500, "Error al guardar el PDF en la base de datos.");
                }

                return Ok(resultado);
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
    }
}
