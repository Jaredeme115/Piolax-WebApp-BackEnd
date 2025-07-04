using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class EtapaComentarioController(IEtapaComentarioService service) : BaseApiController
    {
        private readonly IEtapaComentarioService _service = service;

        [HttpGet("Consultar")]
        public async Task<ActionResult<EtapaComentarioDTO?>> Consultar(int id)
            => await _service.GetByIdAsync(id) is EtapaComentarioDTO dto ? Ok(dto) : NotFound();

        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<EtapaComentarioDTO>>> ConsultarTodos()
        => Ok(await _service.GetAllAsync());
        [HttpPost("Registro")]
        public async Task<ActionResult<EtapaComentarioDTO>> Registro(EtapaComentarioDTO dto)
        => Ok(await _service.CreateAsync(dto));

        [HttpPut("Modificar")]
        public async Task<ActionResult> Modificar(int id, EtapaComentarioDTO dto)
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
