using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class TurnosController(ITurnosService service) : BaseApiController
    {
        private readonly ITurnosService _service = service;

        [Authorize]
        [HttpGet("Consultar")]
        public ActionResult<Turnos?> Consultar(int idTurno)
        {
            return _service.Consultar(idTurno).Result;
        }

        [Authorize]
        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<Turnos>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }

        [Authorize]
        [HttpPost("Registro")]
        public async Task<ActionResult<Turnos>> Registro(TurnoDTO turno)
        {
            if (await _service.TurnoExisteRegistro(turno.descripcion))
            {
                return BadRequest("El turno ya esta registrado");
            }

            return Ok(await _service.Registro(turno));
        }

        [Authorize]
        [HttpPut("Modificar")]
        public async Task<ActionResult<Turnos>> Modificar(int idTurno, TurnoDTO turno)
        {
            if (!await _service.TurnoExiste(idTurno))
            {
                return NotFound("El turno no existe");
            }

            var turnoModificado = await _service.Modificar(idTurno, turno);
            return Ok(turnoModificado);
        }

        [Authorize]
        [HttpDelete("Eliminar")]
        public async Task<ActionResult<Turnos>> Eliminar(int idTurno)
        {
            if (!await _service.TurnoExiste(idTurno))
            {
                return NotFound("El turno no existe");
            }

            return Ok(await _service.Eliminar(idTurno));
        }

    }
}
