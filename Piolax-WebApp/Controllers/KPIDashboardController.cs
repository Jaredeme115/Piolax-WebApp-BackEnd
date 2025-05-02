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
        public async Task<IActionResult> GetMTTA(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var result = await _service.ObtenerMTTA(idArea, idMaquina, anio, mes, semana, diaSemana);
            return Ok(result);
        }
        // Método para obtener MTTA segmentado
        [HttpGet("ObtenerMTTASegmentado")]
        public async Task<ActionResult<List<KpiSegmentadoDTO>>> ObtenerMTTASegmentado(
            int? idArea = null,
            int? idMaquina = null,
            int? anio = null,
            int? mes = null,
            int? semana = null,
            int? diaSemana = null)
        {
            var resultado = await _service.ObtenerMTTASegmentado(idArea, idMaquina, anio, mes, semana, diaSemana);
            return Ok(resultado);
        }

        [HttpGet("mttr")]
        public async Task<IActionResult> GetMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null, int? anio = null, int? mes = null,
             int? semana = null, int? diaSemana = null)
        {
            var result = await _service.ObtenerMTTR(idArea, idMaquina, idEmpleado, anio, mes, semana , diaSemana);
            return Ok(result);
        }

        // Método para obtener MTTR segmentado 
        [HttpGet("ObtenerMTTRSegmentado")]
        public async Task<ActionResult<List<KpiSegmentadoDTO>>> ObtenerMTTRSegmentado(
            int? idArea = null,
            int? idMaquina = null,
            int? idEmpleado = null,
            int? anio = null,
            int? mes = null,
            int? semana = null,
            int? diaSemana = null)
        {
            var resultado = await _service.ObtenerMTTRSegmentado(idArea, idMaquina, idEmpleado, anio, mes, semana, diaSemana);
            return Ok(resultado);
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
            int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var result = await _service.ObtenerTotalDowntime(
                idArea, idMaquina, anio, mes, semana, diaSemana);
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
