using Piolax_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Controllers
{
    public class RolesController(IRolesService service, IEmpleadoService empleadoService ,IEmpleadoAreaRolService empleadoAreaRolService) : BaseApiController
    {
        private readonly IRolesService _service = service;
        private readonly IEmpleadoAreaRolService _empleadoAreaRolService = empleadoAreaRolService;
        private readonly IEmpleadoService _empleadoService = empleadoService;

        //[Authorize]
        [HttpGet("Consultar")]
        public ActionResult<Roles?> Consultar(int idRol)
        {
            return _service.Consultar(idRol).Result;
        }

        [Authorize]
        [HttpGet("ConsultarTodos")]
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

      
        [HttpGet("ObtenerRolPorEmpleadoYArea")]
        public async Task<ActionResult<string>> ObtenerRolPorEmpleadoYArea(string numNomina, int idArea)
        {
            var rol = await _empleadoAreaRolService.ObtenerRolPorEmpleadoYArea(numNomina, idArea);
            if (rol == null)
            {
                return NotFound("No se encontró el rol para el empleado en el área especificada.");
            }

            return Ok(rol);
        }

        [HttpGet("ObtenerRolesPorEmpleado/{numNomina}")]
        public async Task<IActionResult> ObtenerRolesPorEmpleado(string numNomina)
        {
            var roles = await _empleadoAreaRolService.ObtenerRolesPorEmpleado(numNomina);
            if (roles == null || !roles.Any())
            {
                return NotFound("No se encontraron roles para este empleado.");
            }
            return Ok(roles);
        }



    }
}
