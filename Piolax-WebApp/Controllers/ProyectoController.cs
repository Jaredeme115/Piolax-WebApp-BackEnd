using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class ProyectoController(IProyectoService service) : BaseApiController
    {
        private readonly IProyectoService _service = service;

        [HttpGet("Consultar")]
        public async Task<ActionResult<ProyectoDTO?>> Consultar(int id)
            => await _service.GetByIdAsync(id) is ProyectoDTO dto ? Ok(dto) : NotFound();

        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<ProyectoDTO>>> ConsultarTodos()
            => Ok(await _service.GetAllAsync());

        [HttpPost("Registro")]
        public async Task<ActionResult<ProyectoDTO>> Registro(ProyectoDTO dto)
            => Ok(await _service.CreateAsync(dto));

        [HttpPut("Modificar")]
        public async Task<ActionResult> Modificar(int id, ProyectoDTO dto)
        {
            if (!await _service.UpdateAsync(id, dto)) return NotFound();
            return NoContent();
        }

        [HttpDelete("Eliminar")]
        public async Task<ActionResult> Eliminar(int id)
        {
            if (!await _service.DeleteAsync(id)) return NotFound();
            return NoContent();
        }
    }

}
