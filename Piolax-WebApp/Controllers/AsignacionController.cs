using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class AsignacionController(IAsignacionService service) : BaseApiController
    {
        private readonly IAsignacionService _service = service;

        
        [HttpPost("AgregarAsignacion")]
        public async Task<ActionResult<Asignaciones>> AgregarAsignacion(AsignacionesDTO asignacionesDTO)
        {
            return await _service.AgregarAsignacion(asignacionesDTO);
        }

        [HttpGet("ConsultarTodasLasAsignaciones")]
        public async Task<ActionResult<IEnumerable<Asignaciones>>> ConsultarTodasLasAsignaciones()
        {
            return Ok(await _service.ConsultarTodasLasAsignaciones());
        }

        [HttpGet("ConsultarAsignacionPorId")]
        public async Task<ActionResult<Asignaciones>> ConsultarAsignacionPorId(int idAsignacion)
        {
            return await _service.ConsultarAsignacionPorId(idAsignacion);
        }

        [HttpPut("ModificarAsignacion")]
        public async Task<ActionResult<Asignaciones>> ActualizarAsignacion(int idAsignacion, AsignacionesDTO asignacionesDTO)
        {
            return await _service.ActualizarAsignacion(idAsignacion, asignacionesDTO);
        }

        [HttpDelete("EliminarAsignacion")]
        public async Task<ActionResult<Asignaciones>> EliminarAsignacion(int idAsignacion)
        {
            return Ok(await _service.EliminarAsignacion(idAsignacion));
        }

        [HttpGet("ConsultarAsignacionConDetallesPorId")]
        public async Task<ActionResult<AsignacionDetallesDTO?>> ConsultarAsignacionConDetallesPorId(int idAsignacion)
        {
            return await _service.ConsultarAsignacionConDetallesPorId(idAsignacion);
        }


    }
}
