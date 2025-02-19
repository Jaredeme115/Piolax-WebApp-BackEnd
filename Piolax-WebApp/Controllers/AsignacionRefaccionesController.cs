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

        [HttpDelete("EliminarRefaccionDeAsignacion")]
        public async Task<ActionResult<bool>> EliminarRefaccionDeAsignacion(int idAsignacionRefaccion)
        {
            return await _service.EliminarRefaccionDeAsignacion(idAsignacionRefaccion);
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
    }
}
