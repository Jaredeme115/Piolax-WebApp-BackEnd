using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionesController : ControllerBase
    {
        private readonly IAsignacionesService _asignacionesService;

        public AsignacionesController(IAsignacionesService asignacionesService)
        {
            _asignacionesService = asignacionesService;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarAsignacion([FromBody] AsignacionesDTO asignacionesDTO)
        {
            try
            {
                var asignacion = await _asignacionesService.RegistrarAsignacion(asignacionesDTO);
                return CreatedAtAction(nameof(ObtenerAsignacionConDetalles), new { id = asignacion.idAsignacion }, asignacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        // GET: api/Asignaciones/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerAsignacionConDetalles(int id)
        {
            try
            {
                var asignacion = await _asignacionesService.ObtenerAsignacionConDetalles(id);
                if (asignacion == null)
                {
                    return NotFound(new { message = "Asignación no encontrada" });
                }

                return Ok(asignacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Asignaciones
        [HttpGet]
        public async Task<IActionResult> ObtenerTodasLasAsignaciones()
        {
            try
            {
                var asignaciones = await _asignacionesService.ObtenerTodasLasAsignaciones();
                return Ok(asignaciones);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Asignaciones/Tecnico/{numNomina}
        [HttpGet("Tecnico/{numNomina}")]
        public async Task<IActionResult> ObtenerAsignacionPorTecnico(string numNomina)
        {
            try
            {
                var asignaciones = await _asignacionesService.ObtenerAsignacionPorTecnico(numNomina);
                return Ok(asignaciones);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT: api/Asignaciones/{id}/Status
        [HttpPut("{id}/Status")]
        public async Task<IActionResult> ModificarEstatusAprobacionTecnico(int id, [FromBody] int idStatusAprobacionTecnico)
        {
            try
            {
                var asignacion = await _asignacionesService.ModificarEstatusAprobacionTecnico(id, idStatusAprobacionTecnico);
                return Ok(asignacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
