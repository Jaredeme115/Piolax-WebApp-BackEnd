using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class ProyectoFirmaController(IProyectoFirmaService service) : BaseApiController
    {
        private readonly IProyectoFirmaService _service = service;

        [HttpGet("Consultar")]
        public async Task<ActionResult<ProyectoFirmaDTO?>> Consultar(int id)
            => await _service.GetByIdAsync(id) is ProyectoFirmaDTO dto ? Ok(dto) : NotFound();

        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<ProyectoFirmaDTO>>> ConsultarTodos()
        => Ok(await _service.GetAllAsync());

        [HttpPost("Registro")]
        public async Task<ActionResult<ProyectoFirmaDTO>> Registro(ProyectoFirmaDTO dto)
        => Ok(await _service.CreateAsync(dto));

        [HttpPut("Modificar")]
        public async Task<ActionResult> Modificar(int id, ProyectoFirmaDTO dto)
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
