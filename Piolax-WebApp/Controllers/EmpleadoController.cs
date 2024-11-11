using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class EmpleadoController(IEmpleadoService service, ITokenService token) : BaseApiController
    {
        private readonly IEmpleadoService _service = service;
        private readonly ITokenService _tokenService = token;


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

            return Ok(await _service.Registro(registro));
        }

        [Authorize]
        [HttpPut("{numNomina}")]
        public async Task<ActionResult<Empleado>> Modificar(string numNomina, RegistroDTO registro)
        {
            if (!await _service.EmpleadoExiste(registro.numNomina))
            {
                return NotFound("El empleado no existe");
            }

            var empleadoModificado = await _service.Modificar(numNomina, registro);
            return Ok(empleadoModificado);
        }

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


        [HttpPost("Login")]
        public async Task<ActionResult<EmpleadoDTO>> Login(LoginDTO login)
        {
            if (!await _service.EmpleadoExiste(login.numNomina))
                return Unauthorized("El Empleado no existe");

            var resultado = _service.EmpleadoExisteLogin(login);

            if (!resultado.esLoginExitoso)
                return Unauthorized("Password no valido");

            var empleadoDTO = new EmpleadoDTO
            {
                numNomina = resultado.empleado.numNomina,
                token = _tokenService.CrearToken(resultado.empleado)
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
    }
}
