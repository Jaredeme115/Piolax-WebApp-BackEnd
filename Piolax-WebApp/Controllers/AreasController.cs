using Piolax_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Controllers
{
    public class AreasController(IAreasService service) : BaseApiController
    {
        private readonly IAreasService _service = service;

        [Authorize]
        [HttpGet("Consultar")]
        public ActionResult<Areas?> Consultar(int idArea)
        {
            return _service.Consultar(idArea).Result;
        }

        [Authorize]
        [HttpGet("Consultar Todos")]
        public async Task<ActionResult<IEnumerable<Areas>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }

        [Authorize]
        [HttpPost("Registro")]

        public async Task<ActionResult<Areas>> Registro(AreaDTO area)
        {
            if (await _service.AreaExisteRegistro(area.nombreArea))
            {
                return BadRequest("El area ya esta registrada");
            }

            return Ok(await _service.Registro(area));
        }

        [Authorize]
        [HttpPut("Modificar")]
        public async Task<ActionResult<Areas>> Modificar(int idArea, AreaDTO area)
        {
            if (!await _service.AreaExiste(idArea))
            {
                return NotFound("El area no existe");
            }

            var areaModificada = await _service.Modificar(idArea, area);
            return Ok(areaModificada);
        }

        [Authorize]
        [HttpDelete("Eliminar")]
        public async Task<ActionResult<Areas>> Eliminar(int idArea)
        {
            if (!await _service.AreaExiste(idArea))
            {
                return NotFound("El area no existe");
            }

            return Ok(await _service.Eliminar(idArea));
        }
    }
}
