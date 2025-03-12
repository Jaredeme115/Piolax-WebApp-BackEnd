using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;

namespace Piolax_WebApp.Controllers
{
    public class EstatusPreventivoController(IEstatusPreventivoService service) : BaseApiController
    {
        private readonly IEstatusPreventivoService _service = service;

        [HttpGet("ConsultarEstatusPreventivoPorID/{id}")]
        public async Task<ActionResult<EstatusPreventivo>> GetEstatusPreventivoById(int id)
        {
            var estatusPreventivo = await _service.ConsultarEstatusPreventivoPorID(id);
            if (estatusPreventivo == null)
            {
                return NotFound();
            }
            return Ok(estatusPreventivo);
        }

        [HttpGet("ConsultarTodosLosEstatusMP")]
        public async Task<ActionResult<IEnumerable<EstatusPreventivo>>> GetAllEstatusPreventivo()
        {
            var estatusPreventivo = await _service.ConsultarTodosEstatusPreventivo();
            return Ok(estatusPreventivo);
        }
    }
}
