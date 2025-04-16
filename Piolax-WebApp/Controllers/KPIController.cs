using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KPIController : ControllerBase
    {
        private readonly IKPIService _kpiService;

        public KPIController(IKPIService kpiService)
        {
            _kpiService = kpiService;
        }

        [HttpPost("obtener-kpis")]
        public async Task<IActionResult> ObtenerKPIs([FromBody] KPIFiltroDTO filtros)
        {
            var datos = await _kpiService.ObtenerKPIs(filtros);
            return Ok(datos);
        }
        [HttpPost("FiltrarKPIs")]
        public async Task<ActionResult<List<KpiDashboardDTO>>> FiltrarKPIs([FromBody] KPIFiltroDTO filtros)
        {
            var resultado = await _kpiService.ObtenerKPIs(filtros);
            return Ok(resultado);
        }

    }

}
