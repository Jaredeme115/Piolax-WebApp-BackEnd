using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;

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
        public async Task<ActionResult<Asignacion_Tecnico>> CrearAsignacionTecnico(Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            return await _service.CrearAsignacionTecnico(asignacionTecnicoDTO);
        }

        [HttpPost("FinalizarAsignacionTecnico")]
        public async Task<ActionResult<Asignacion_Tecnico>> FinalizarAsignacionTecnico(Asignacion_TecnicoFinalizacionDTO asignacionTecnicoFinalizacionDTO)
        {
            return await _service.FinalizarAsignacionTecnico(asignacionTecnicoFinalizacionDTO);
        }

        [HttpDelete("EliminarTecnicoDeAsignacion")]
        public async Task<ActionResult<bool>> EliminarTecnicoDeAsignacion(int idAsignacionTecnico)
        {
            return await _service.EliminarTecnicoDeAsignacion(idAsignacionTecnico);
        }

        [HttpGet("ConsultarTodosLosTecnicos")]
        public async Task<ActionResult<IEnumerable<Asignacion_Tecnico>>> ConsultarTodosLosTecnicos()
        {
            return Ok(await _service.ConsultarTodosLosTecnicos());
        }

        [HttpPut("ActualizarTecnicoEnAsignacion")]
        public async Task<ActionResult<bool>> ActualizarTecnicoEnAsignacion(Asignacion_TecnicoDTO asignacionTecnicoDTO)
        {
            return await _service.ActualizarTecnicoEnAsignacion(asignacionTecnicoDTO);
        }

        [HttpGet("ConsultarTecnicosConDetallesPorAsignacion")]
        public async Task<ActionResult<IEnumerable<Asignacion_TecnicoDetallesDTO>>> ConsultarTecnicosConDetallesPorAsignacion(int idAsignacion)
        {
            return Ok(await _service.ConsultarTecnicosConDetallesPorAsignacion(idAsignacion));
        }
    }
}
