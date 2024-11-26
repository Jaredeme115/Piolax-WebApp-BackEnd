using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class StatusAprobacionSolicitanteController(IStatusAprobacionSolicitanteService service) : BaseApiController
    {
        private readonly IStatusAprobacionSolicitanteService _service = service;

        [Authorize]
        [HttpGet("Consultar")]
        public ActionResult<StatusAprobacionSolicitante?> Consultar(int idStatusAprobacionSolicitante)
        {
            return _service.Consultar(idStatusAprobacionSolicitante).Result;
        }

        [Authorize]
        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<StatusAprobacionSolicitante>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }

        [Authorize]
        [HttpPost("Registro")]
        public async Task<ActionResult<StatusAprobacionSolicitante>> Registro(StatusAprobacionSolicitanteDTO statusAprobacionSolicitante)
        {
            if (await _service.StatusAprobacionSolicitanteExisteRegistro(statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante))
            {
                return BadRequest("El status de aprobacion de solicitante ya esta registrado");
            }

            return Ok(await _service.Registro(statusAprobacionSolicitante));
        }

        [Authorize]
        [HttpPut("Modificar")]
        public async Task<ActionResult<StatusAprobacionSolicitante>> Modificar(int idStatusAprobacionSolicitante, StatusAprobacionSolicitanteDTO statusAprobacionSolicitante)
        {
            if (!await _service.StatusAprobacionSolicitanteExiste(idStatusAprobacionSolicitante))
            {
                return NotFound("El status de aprobacion de solicitante no existe");
            }

            var statusAprobacionSolicitanteModificado = await _service.Modificar(idStatusAprobacionSolicitante, statusAprobacionSolicitante);
            return Ok(statusAprobacionSolicitanteModificado);
        }


        [Authorize]
        [HttpDelete("Eliminar")]
        public async Task<ActionResult<StatusAprobacionSolicitante>> Eliminar(int idStatusAprobacionSolicitante)
        {
            if (!await _service.StatusAprobacionSolicitanteExiste(idStatusAprobacionSolicitante))
            {
                return NotFound("El status de aprobacion de solicitante no existe");
            }

            return Ok(await _service.Eliminar(idStatusAprobacionSolicitante));
        }


    }
}
