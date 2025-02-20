using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;

namespace Piolax_WebApp.Controllers
{
    public class AsignacionTecnicosController(IAsignacionTecnicosService service) : BaseApiController
    {
        private readonly IAsignacionTecnicosService _service = service;

        [HttpGet("ConsultarTecnicosPorAsignacion")]
        public async Task<ActionResult<IEnumerable<Asignacion_TecnicoDetallesDTO>>> ConsultarTecnicosPorAsignacion(int idAsignacion)
        {
            return Ok(await _service.ConsultarTecnicosPorAsignacion(idAsignacion));
        }

        [HttpPost("CrearAsignacionTecnico")]
        public async Task<ActionResult<Asignacion_TecnicoResponseDTO?>> CrearAsignacionTecnico([FromBody] Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            return await _service.CrearAsignacionTecnico(asignacionTecnicoDTO);
        }

        [HttpPost("FinalizarAsignacionTecnico")]
        public async Task<ActionResult<Asignacion_TecnicoFinalizacionResponseDTO>> FinalizarAsignacionTecnico([FromBody] Asignacion_TecnicoFinalizacionDTO asignacionTecnicoFinalizacionDTO)
        {
            return await _service.FinalizarAsignacionTecnico(asignacionTecnicoFinalizacionDTO);
        }

        [HttpPost("PausarAsignacion")]
        public async Task<ActionResult<bool>> PausarAsignacion([FromBody] PausarAsignacionDTO pausarAsignacionDTO)
        {
            return await _service.PausarAsignacion(pausarAsignacionDTO.idAsignacion, pausarAsignacionDTO.idTecnicoQuePausa, pausarAsignacionDTO.comentarioPausa);
        }

        [HttpPost("RetirarTecnicoDeApoyo")]
        public async Task<ActionResult<bool>> RetirarTecnicoDeApoyo([FromBody] RetirarTecnicoDTO retirarTecnicoDTO)
        {
            return await _service.RetirarTecnicoDeApoyo(retirarTecnicoDTO.idAsignacion, retirarTecnicoDTO.idTecnicoQueSeRetira, retirarTecnicoDTO.comentarioRetiro);
        }

        [HttpDelete("EliminarTecnicoDeAsignacion")]
        public async Task<ActionResult<bool>> EliminarTecnicoDeAsignacion([FromBody] EliminarTecnicoDTO eliminarTecnicoDTO)
        {
            return await _service.EliminarTecnicoDeAsignacion(eliminarTecnicoDTO.idAsignacionTecnico);
        }

        [HttpGet("ConsultarTodosLosTecnicos")]
        public async Task<ActionResult<IEnumerable<Asignacion_Tecnico>>> ConsultarTodosLosTecnicos()
        {
            return Ok(await _service.ConsultarTodosLosTecnicos());
        }

        [HttpPut("ActualizarTecnicoEnAsignacion")]
        public async Task<ActionResult<bool>> ActualizarTecnicoEnAsignacion([FromBody] Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            return await _service.ActualizarTecnicoEnAsignacion(asignacionTecnicoDTO);
        }

        [HttpGet("ConsultarTecnicosConDetallesPorAsignacion")]
        public async Task<ActionResult<IEnumerable<Asignacion_TecnicoDetallesDTO>>> ConsultarTecnicosConDetallesPorAsignacion(int idAsignacion)
        {
            return Ok(await _service.ConsultarTecnicosConDetallesPorAsignacion(idAsignacion));
        }

        [HttpGet("OrdenesPausadasPorTecnico")]
        public async Task<ActionResult<IEnumerable<Asignacion_TecnicoDetallesDTO>>> OrdenesPausadasPorTecnico(int idEmpleado)
        {
            var ordenesPausadas = await _service.ConsultarOrdenesEnPausaPorTecnico(idEmpleado);

            if (!ordenesPausadas.Any())
                return NotFound("No se encontraron órdenes pausadas para este técnico.");

            return Ok(ordenesPausadas);
        }
    }
}
