using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class AsignacionRefaccionesController(IAsignacionRefaccionesService service) : BaseApiController
    {
        private readonly IAsignacionRefaccionesService _service = service;

        [HttpPost("CrearAsignacionRefacciones")]
        public async Task<ActionResult<Asignacion_RefaccionesResponseDTO>> CrearAsignacionRefacciones([FromBody] Asignacion_RefaccionesDTO asignacionRefaccionesDTO)
        {
            return await _service.CrearAsignacionRefacciones(asignacionRefaccionesDTO);
        }

        [HttpGet("ConsultarTodasLasRefacciones")]
        public async Task<ActionResult<IEnumerable<Asignacion_RefaccionesDetallesDTO>>> ConsultarTodasLasRefacciones()
        {
            return Ok(await _service.ConsultarTodasLasRefacciones());
        }

        /*[HttpDelete("EliminarRefaccionDeAsignacion")]
        public async Task<ActionResult<bool>> EliminarRefaccionDeAsignacion([FromBody] EliminarRefaccionDTO eliminarRefaccionDTO)
        {
            return await _service.EliminarRefaccionDeAsignacion(eliminarRefaccionDTO.idAsignacionRefaccion,);
        }*/

        [HttpDelete("EliminarRefaccionDeAsignacion")]
        public async Task<ActionResult<bool>> EliminarRefaccionDeAsignacion([FromBody] EliminarRefaccionDTO eliminarRefaccionDTO)
        {
            try
            {
                bool resultado = await _service.EliminarRefaccionDeAsignacion(
                    eliminarRefaccionDTO.idAsignacionRefaccion,
                    eliminarRefaccionDTO.idAsignacionActual
                );

                if (!resultado)
                {
                    return NotFound("La refaccion no fue encontrada o no se puede eliminar");
                }
                return Ok(true);
            }

            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno en el servidor: {ex.Message}");
            }


        }

        [HttpPut("ActualizarRefaccionEnAsignacion")]
        public async Task<ActionResult<bool>> ActualizarRefaccionEnAsignacion([FromBody] Asignacion_RefaccionesDTO asignacionRefaccionesDTO)
        {
            return await _service.ActualizarRefaccionEnAsignacion(asignacionRefaccionesDTO);
        }

        [HttpGet("ConsultarRefaccionesConDetallesPorAsignacion")]
        public async Task<ActionResult<IEnumerable<Asignacion_RefaccionesDetallesDTO>>> ConsultarRefaccionesConDetallesPorAsignacion(int idAsignacion)
        {
            return Ok(await _service.ConsultarRefaccionesConDetallesPorAsignacion(idAsignacion));
        }

        [HttpGet("ConsultarRefaccionesPorSolicitud/{idSolicitud}")]
        public async Task<ActionResult<IEnumerable<Asignacion_RefaccionesDetallesDTO>>> ConsultarRefaccionesPorSolicitud(int idSolicitud)
        {
            var resultado = await _service.ConsultarRefaccionesPorSolicitud(idSolicitud);
            return Ok(resultado);
        }

    }
}
