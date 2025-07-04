
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class EtapaActividadController(IEtapaActividadService service) : BaseApiController
    {
        private readonly IEtapaActividadService _service = service;

        [HttpGet("Consultar")]
        public async Task<ActionResult<EtapaActividadDTO?>> Consultar(int id)
            => await _service.GetByIdAsync(id) is EtapaActividadDTO dto ? Ok(dto) : NotFound();

        [HttpGet("ConsultarTodos")]
        public async Task<ActionResult<IEnumerable<EtapaActividadDTO>>> ConsultarTodos()
        => Ok(await _service.GetAllAsync());
        [HttpPost("Registro")]
        public async Task<ActionResult<EtapaActividadDTO>> Registro(EtapaActividadDTO dto)
        => Ok(await _service.CreateAsync(dto));

        [HttpPut("Modificar")]
        public async Task<ActionResult> Modificar(int id, EtapaActividadDTO dto)
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
