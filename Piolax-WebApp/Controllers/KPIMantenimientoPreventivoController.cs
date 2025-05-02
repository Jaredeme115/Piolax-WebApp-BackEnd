using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class KPIMantenimientoPreventivoController(IKPIMantenimientoPreventivoService service) : BaseApiController
    {
        private readonly IKPIMantenimientoPreventivoService _service = service;

        // Endpoint para calcular KPIs
        [HttpPost("calcular")]
        public async Task<IActionResult> CalcularKPIs([FromBody] PeriodoDTO periodo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _service.CalcularYGuardarKPIs(periodo.FechaInicio, periodo.FechaFin);
                return Ok(new
                {
                    success = true,
                    message = "KPIs calculados y guardados exitosamente",
                    periodo = new
                    {
                        inicio = periodo.FechaInicio,
                        fin = periodo.FechaFin
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al calcular los KPIs",
                    error = ex.Message
                });
            }
        }

        // Endpoint para calcular KPIs del mes actual
        [HttpPost("calcular-mes-actual")]
        public async Task<IActionResult> CalcularKPIsMesActual()
        {
            try
            {
                DateTime hoy = DateTime.Today;
                DateTime inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
                DateTime finMes = inicioMes.AddMonths(1).AddDays(-1);

                await _service.CalcularYGuardarKPIs(inicioMes, finMes);
                return Ok(new
                {
                    success = true,
                    message = "KPIs del mes actual calculados y guardados exitosamente",
                    periodo = new
                    {
                        inicio = inicioMes,
                        fin = finMes
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al calcular los KPIs del mes actual",
                    error = ex.Message
                });
            }
        }

        // Endpoints para consultar KPIs

        [HttpGet("cumplimiento")]
        public async Task<IActionResult> GetCumplimiento(int? anio = null, int? mes = null)
        {
            var result = await _service.ObtenerCumplimiento(anio, mes);
            return Ok(result);
        }

        [HttpGet("efectividad")]
        public async Task<IActionResult> GetEfectividad(int? anio = null, int? mes = null)
        {
            var result = await _service.ObtenerEfectividad(anio, mes);
            return Ok(result);
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumenKPIs(int? anio = null, int? mes = null)
        {
            var result = await _service.ObtenerResumenKPIsPreventivo(anio, mes);
            return Ok(result);
        }

        /// <summary>
        /// Devuelve los contadores de mantenimientos preventivos por estatus
        /// </summary>
        [HttpGet("GetContadoresMP")]
        public async Task<ActionResult<ContadoresMPDTO>> GetContadoresMP(int? anio, int? mes)
        {
            var resultado = await _service.ObtenerContadoresMP(anio, mes);
            return Ok(resultado);
        }

    }
}