using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class MantenimientoPreventivoEjecucionController(IMantenimientoPreventivoEjecucionService service): BaseApiController
    {
        private readonly IMantenimientoPreventivoEjecucionService _service = service;

        [HttpPost("CrearEjecucion")]
        public async Task<ActionResult<MantenimientoPreventivoEjecucionDTO>>
          CrearEjecucion([FromBody] MantenimientoPreventivoEjecucionCrearDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CrearEjecucionAsync(dto);
            return CreatedAtAction(nameof(CrearEjecucion), new { id = result.idMPEjecucion }, result);
        }


        [HttpGet("EjecucionesPorMP/{idMP}")]
        public async Task<ActionResult<IEnumerable<MantenimientoPreventivoEjecucionDTO>>> EjecucionesPorMP(int idMP)
        {
            var list = await _service.ObtenerEjecucionesPorMPAsync(idMP);
            return Ok(list);
        }
    }
}
