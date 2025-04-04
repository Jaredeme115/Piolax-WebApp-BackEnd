using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.DTOs.Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("ObtenerFiltrosDashboard")]
        public async Task<ActionResult<FiltrosDashboardDTO>> ObtenerFiltrosDashboard()
        {
            var filtros = await _dashboardService.ObtenerFiltrosDashboardAsync();
            return Ok(filtros);
        }

        [HttpPost("IndicadoresDashboard")]
        public async Task<ActionResult<List<KpiDashboardDTO>>> IndicadoresDashboard([FromBody] FiltroDashboardDTO filtro)
        {
            var datos = await _dashboardService.ObtenerIndicadoresDashboardAsync(filtro);
            return Ok(datos);
        }

    }


}
