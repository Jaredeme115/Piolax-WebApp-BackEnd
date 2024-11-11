using Piolax_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Controllers
{
    public class RolesController(IRolesService service) : BaseApiController
    {
        private readonly IRolesService _service = service;

        [Authorize]
        [HttpGet("Consultar")]
        public ActionResult<Roles?> Consultar(int idRol)
        {
            return _service.Consultar(idRol).Result;
        }

        [Authorize]
        [HttpGet("Consultar Todos")]
        public async Task<ActionResult<IEnumerable<Roles>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }

        [Authorize]
        [HttpPost("Registro")]
        public async Task<ActionResult<Roles>> Registro(RolDTO rol)
        {
            if (await _service.RolExisteRegistro(rol.nombreRol))
            {
                return BadRequest("El rol ya esta registrado");
            }

            return Ok(await _service.Registro(rol));
        }

        [Authorize]
        [HttpPut("Modificar")]
        public async Task<ActionResult<Roles>> Modificar(int idRol, RolDTO rol)
        {
            if (!await _service.RolExiste(idRol))
            {
                return NotFound("El rol no existe");
            }

            var rolModificado = await _service.Modificar(idRol, rol);
            return Ok(rolModificado);
        }

        [Authorize]
        [HttpDelete("Eliminar")]
        public async Task<ActionResult<Roles>> Eliminar(int idRol)
        {
            if (!await _service.RolExiste(idRol))
            {
                return NotFound("El rol no existe");
            }

            return Ok(await _service.Eliminar(idRol));
        }

    }
}
