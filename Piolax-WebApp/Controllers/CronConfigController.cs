using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.Services;
using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Controllers
{
    public class CronConfigController(ICronConfigService service): BaseApiController
    {
        private readonly ICronConfigService _service = service;

        [HttpGet("ConsultarTodosCron")]
        public async Task<ActionResult<IEnumerable<CronConfigDTO>>> ConsultarTodos()
        {
            var list = await _service.ConsultarTodos();
            return Ok(list);
        }

        [HttpGet("{nombreCronConfig}")]
        public async Task<ActionResult<CronConfigDTO>> Consultar([FromRoute] string nombreCronConfig)
        {
            var cfg = await _service.Consultar(nombreCronConfig);
            return Ok(cfg);
        }

        [HttpPut("{nombreCronConfig}")]
        public async Task<IActionResult> Actualizar(
            [FromRoute] string nombreCronConfig,
            [FromBody] UpdateCronConfigDTO updateCronConfigDTO)
        {
            var updated = await _service.Actualizar(nombreCronConfig, updateCronConfigDTO);
            return Ok(updated);
        }
    }
}

