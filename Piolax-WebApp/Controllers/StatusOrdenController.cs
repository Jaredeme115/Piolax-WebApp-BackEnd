using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class StatusOrdenController(IStatusOrdenService service) : BaseApiController
    {
        private readonly IStatusOrdenService _service = service;

        [Authorize]
        [HttpGet("Consultar")]
        public ActionResult<StatusOrden?> Consultar(int idStatusOrden)
        {
            return _service.Consultar(idStatusOrden).Result;
        }

        [Authorize]
        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<StatusOrden>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }

        [Authorize]
        [HttpPost("Registro")]
        public async Task<ActionResult<StatusOrden>> Registro(StatusOrdenDTO statusOrden)
        {
            if (await _service.StatusOrdenExisteRegistro(statusOrden.descripcionStatusOrden))
            {
                return BadRequest("El status de orden ya esta registrado");
            }

            return Ok(await _service.Registro(statusOrden));
        }

        [Authorize]
        [HttpPut("Modificar")]
        public async Task<ActionResult<StatusOrden>> Modificar(int idStatusOrden, StatusOrdenDTO statusOrden)
        {
            if (!await _service.StatusOrdenExiste(idStatusOrden))
            {
                return NotFound("El status de orden no existe");
            }

            var statusOrdenModificado = await _service.Modificar(idStatusOrden, statusOrden);
            return Ok(statusOrdenModificado);
        }

        [Authorize]
        [HttpDelete("Eliminar")]
        public async Task<ActionResult<StatusOrden>> Eliminar(int idStatusOrden)
        {
            if (!await _service.StatusOrdenExiste(idStatusOrden))
            {
                return NotFound("El status de orden no existe");
            }

            return Ok(await _service.Eliminar(idStatusOrden));
        }


    }
}
