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
        [HttpGet]
        [Route("Consultar")]
        public ActionResult<Empleado> Consultar(string numNomina)
        {
            return _service.Consultar(numNomina).Result;
        }

        [Authorize]
        [HttpGet]
        [Route("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<Empleado>>> ConsultarTodos()
        {
            return Ok(await _service.ConsultarTodos());
        }

        [HttpPost("registro")]
        public async Task<ActionResult<Empleado>> Registro(RegistroDTO registro)
        {
            if (await _service.EmpleadoExiste(registro.numNomina))
            {
                return BadRequest("El numero de nomina ya esta registrado");

            }

            return Ok(await _service.Registro(registro));
        }

        [HttpPost("login")]
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
    }
}
