using Piolax_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Controllers
{
    public class StatusEmpleadoController(IStatusEmpleadoService service) : BaseApiController
    {
        private readonly IStatusEmpleadoService _service = service;

        //[Authorize]
        [HttpGet("Consultar")]
        public ActionResult<StatusEmpleado?> Consultar(int idStatusEmpleado)
        {
            return _service.Consultar(idStatusEmpleado).Result;
        }

        //[Authorize]
        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<StatusEmpleado>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }

        //[Authorize]
        [HttpPost("Registro")]
        public async Task<ActionResult<StatusEmpleado>> Registro(StatusEmpleadoDTO statusEmpleado)
        {
            if (await _service.StatusEmpleadoExisteRegistro(statusEmpleado.descripcionStatusEmpleado))
            {
                return BadRequest("El status de empleado ya esta registrado");
            }

            return Ok(await _service.Registro(statusEmpleado));
        }

        //[Authorize]
        [HttpPut("Modificar")]
        public async Task<ActionResult<StatusEmpleado>> Modificar(int idStatusEmpleado, StatusEmpleadoDTO statusEmpleado)
        {
            if (!await _service.StatusEmpleadoExiste(idStatusEmpleado))
            {
                return NotFound("El status de empleado no existe");
            }

            var statusEmpleadoModificado = await _service.Modificar(idStatusEmpleado, statusEmpleado);
            return Ok(statusEmpleadoModificado);
        }

        //[Authorize]
        [HttpDelete("Eliminar")]
        public async Task<ActionResult<StatusEmpleado>> Eliminar(int idStatusEmpleado)
        {
            if (!await _service.StatusEmpleadoExiste(idStatusEmpleado))
            {
                return NotFound("El status de empleado no existe");
            }

            return Ok(await _service.Eliminar(idStatusEmpleado));
        }
    }
}
