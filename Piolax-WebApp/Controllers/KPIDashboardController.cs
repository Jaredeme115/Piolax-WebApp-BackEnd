using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;

namespace Piolax_WebApp.Controllers
{
    public class KPIDashboardController(IKPIDashboardService service) : BaseApiController
    {
        private readonly IKPIDashboardService _service = service;

        [HttpGet("mtta")]
        public async Task<IActionResult> GetMTTA(int? idArea = null, int? idMaquina = null)
        {
            var result = await _service.ObtenerMTTA(idArea, idMaquina);
            return Ok(result);
        }

        [HttpGet("mttr")]
        public async Task<IActionResult> GetMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null)
        {
            var result = await _service.ObtenerMTTR(idArea, idMaquina, idEmpleado);
            return Ok(result);
        }

        [HttpGet("mtbf")]
        public async Task<IActionResult> GetMTBF(int? idArea = null)
        {
            var result = await _service.ObtenerMTBF(idArea);
            return Ok(result);
        }

        [HttpGet("totaldowntime")]
        public async Task<IActionResult> GetTotalDowntime(
            int? idArea = null, int? idMaquina = null,
            int? año = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var result = await _service.ObtenerTotalDowntime(
                idArea, idMaquina, año, mes, semana, diaSemana);
            return Ok(result);
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumenKPIs(
            int? idAreaMTTA = null, int? idMaquinaMTTA = null,
            int? idAreaMTTR = null, int? idMaquinaMTTR = null, int? idEmpleadoMTTR = null,
            int? idAreaMTBF = null,
            int? idAreaDowntime = null, int? idMaquinaDowntime = null,
            int? añoDowntime = null, int? mesDowntime = null, int? semanaDowntime = null, int? diaSemanaDowntime = null)
        {
            var result = await _service.ObtenerResumenKPIs(
                idAreaMTTA, idMaquinaMTTA,
                idAreaMTTR, idMaquinaMTTR, idEmpleadoMTTR,
                idAreaMTBF,
                idAreaDowntime, idMaquinaDowntime,
                añoDowntime, mesDowntime, semanaDowntime, diaSemanaDowntime);

            return Ok(result);
        }

    }
}
