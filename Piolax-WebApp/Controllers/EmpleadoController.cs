using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Piolax_WebApp.Controllers
{
    public class EmpleadoController(IEmpleadoService service, ITokenService token, IEmpleadoAreaRolService empleadoAreaRolService, IRefreshTokensService refreshTokensService) : BaseApiController
    {
        private readonly IEmpleadoService _service = service;
        private readonly ITokenService _tokenService = token;
        private readonly IEmpleadoAreaRolService _empleadoAreaRolService = empleadoAreaRolService;
        private readonly IRefreshTokensService _refreshTokensService = refreshTokensService;



        [Authorize]
        [HttpGet("Consultar")]
        public ActionResult<Empleado?> Consultar(string numNomina)
        {
            return _service.Consultar(numNomina).Result;
        }

        [Authorize]
        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<Empleado>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }


        [HttpGet("{numNomina}/DetalleConAreasRoles")]
        public async Task<IActionResult> ObtenerDetalleConAreasRoles(string numNomina)
        {
            // Obtener el empleado
            var empleado = await _service.Consultar(numNomina);
            if (empleado == null)
            {
                return NotFound($"No se encontró el empleado con número de nómina: {numNomina}");
            }

            // Obtener las áreas y roles del empleado
            var areasRoles = await _empleadoAreaRolService.ObtenerAreasRolesPorEmpleado(numNomina);
            if (!areasRoles.Any())
            {
                return NotFound($"No se encontraron áreas o roles asignados para el empleado con número de nómina: {numNomina}");
            }

            // Crear el DTO de respuesta
            var empleadoConAreasRolesDTO = new EmpleadoAreaRolDTO
            {
                NumNomina = empleado.numNomina,
                NombreCompleto = $"{empleado.nombre} {empleado.apellidoPaterno} {empleado.apellidoMaterno}",
                AreasRoles = areasRoles.Select(ar => new AreaRolDTO
                {
                    Area = ar.Area.nombreArea,
                    Rol = ar.Rol.nombreRol
                }).ToList()
            };

            return Ok(empleadoConAreasRolesDTO);
        }

        [Authorize]
        [HttpPost("Registro")]
        public async Task<ActionResult<Empleado>> Registro(RegistroDTO registro)
        {
            if (await _service.EmpleadoExiste(registro.numNomina))
            {
                return BadRequest("El numero de nomina ya esta registrado");

            }

            // Asignar el valor 1 al idStatusEmpleado si no se proporciona
            if (registro.idStatusEmpleado == 0)
            {
                registro.idStatusEmpleado = 1;
            }

            // Registrar el empleado junto con área y rol
            try
            {
                await _empleadoAreaRolService.RegistrarEmpleadoConAreaYRol(registro);
                return Ok("Empleado registrado exitosamente con área y rol asignados.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al registrar empleado: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("AsignarAreaRol")]
        public async Task<ActionResult> AsignarAreaRol(string numNomina, int idArea, int idRol)
        {
            try
            {
                // Verifica si el empleado existe
                if (!await _service.EmpleadoExiste(numNomina))
                {
                    return NotFound("El empleado no existe");
                }

                // Asigna el área y rol al empleado
                await _empleadoAreaRolService.AsignarAreaRol(numNomina, idArea, idRol);
                return Ok("Área y rol asignados exitosamente al empleado.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al asignar área y rol al empleado: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("ModificarEmpleadoConAreaYRol/{numNomina}")]
        public async Task<ActionResult> ModificarEmpleadoAreaRol(string numNomina, RegistroDTO registro)
        {
            try
            {
                // Verifica si el empleado existe
                if (!await _service.EmpleadoExiste(numNomina))
                {
                    return NotFound("El empleado no existe");
                }

                // Modifica el empleado y asigna el área y rol
                await _empleadoAreaRolService.ModificarEmpleadoAreaRol(numNomina, registro);
                return Ok("Empleado modificado exitosamente junto con su área y rol.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al modificar el empleado: {ex.Message}");
            }
        }

        /*[Authorize]
        [HttpPut("{numNomina}")]
        public async Task<ActionResult<Empleado>> Modificar(string numNomina, RegistroDTO registro)
        {
            if (!await _service.EmpleadoExiste(registro.numNomina))
            {
                return NotFound("El empleado no existe");
            }

            var empleadoModificado = await _service.Modificar(numNomina, registro);
            return Ok(empleadoModificado);
        }*/

        [Authorize]
        [HttpDelete("{numNomina}")]
        public async Task<ActionResult<Empleado>> Eliminar(string numNomina)
        {
            if (!await _service.EmpleadoExiste(numNomina))
            {
                return NotFound("El empleado no existe");
            }

            return Ok(await _service.Eliminar(numNomina));
        }

        //Login con JWT de Acceso y Refresh
        [HttpPost("Login")]
        public async Task<ActionResult<EmpleadoDTO>> Login([FromBody] LoginDTO login)
        {
            if (!await _service.EmpleadoExiste(login.numNomina))
                return Unauthorized("El Empleado no existe");

            var resultado = _service.EmpleadoExisteLogin(login);

            if (!resultado.esLoginExitoso)
                return Unauthorized("Password no valido");

            var token = _tokenService.CrearToken(resultado.empleado);
            var refreshToken = await _refreshTokensService.GenerateRefreshToken(resultado.empleado.idEmpleado);

            var empleadoDTO = new EmpleadoDTO
            {
                numNomina = resultado.empleado.numNomina,
                token = token,
                refreshToken = refreshToken.token
            };

            return Ok(empleadoDTO);
        }

        [Authorize]
        [HttpGet("ConsultarPorStatus/{idStatusEmpleado}")]
        public async Task<ActionResult<IEnumerable<Empleado>>> ConsultarPorStatus(int idStatusEmpleado)
        {
            var empleados = await _service.ConsultarPorStatus(idStatusEmpleado);
            if (empleados == null || !empleados.Any())
            {
                return NotFound("No se encontraron empleados con el status especificado.");
            }

            return Ok(empleados);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            var refreshToken = await _refreshTokensService.GetRefreshToken(refreshTokenDTO.refreshToken);
            if (refreshToken == null || refreshToken.expiresAt <= DateTime.UtcNow)
            {
                return Unauthorized("Refresh token inválido o expirado");
            }

            var principal = _tokenService.ObtenerClaimsPrincipal(refreshTokenDTO.refreshToken);
            if (principal == null)
            {
                return Unauthorized("Refresh token inválido");
            }

            var numNomina = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var empleado = await _service.Consultar(numNomina);
            if (empleado == null)
            {
                return Unauthorized("Empleado no encontrado");
            }

            var newToken = _tokenService.CrearToken(empleado);
            var newRefreshToken = await _refreshTokensService.GenerateRefreshToken(empleado.idEmpleado);

            await _refreshTokensService.RevokeRefreshToken(refreshTokenDTO.refreshToken);

            return Ok(new
            {
                token = newToken,
                refreshToken = newRefreshToken.token
            });
        }
    }
}
