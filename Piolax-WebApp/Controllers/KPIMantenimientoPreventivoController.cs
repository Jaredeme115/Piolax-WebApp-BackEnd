using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;

namespace Piolax_WebApp.Controllers
{
    public class KPIMantenimientoPreventivoController(IKPIMantenimientoPreventivoService service): BaseApiController
    {
        private readonly IKPIMantenimientoPreventivoService _service = service;

        // Endpoint para calcular KPIs
        [HttpPost("calcular")]
        public async Task<IActionResult> CalcularKPIs([FromBody] PeriodoDTO periodo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (periodo.FechaInicio > periodo.FechaFin)
            {
                return BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");
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
                DateTime hoy = DateTime.Now;
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
        public async Task<IActionResult> GetCumplimiento(int? año = null, int? mes = null)
        {
            var result = await _service.ObtenerCumplimiento(año, mes);
            return Ok(result);
        }

        [HttpGet("efectividad")]
        public async Task<IActionResult> GetEfectividad(int? año = null, int? mes = null)
        {
            var result = await _service.ObtenerEfectividad(año, mes);
            return Ok(result);
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumenKPIs(int? año = null, int? mes = null)
        {
            var result = await _service.ObtenerResumenKPIsPreventivo(año, mes);
            return Ok(result);
        }

        /// <summary>
        /// Devuelve los contadores de mantenimientos preventivos por estatus
        /// </summary>
        [HttpGet("contadores")]
        public async Task<IActionResult> GetContadoresMP(int? año = null, int? mes = null)
        {
            try
            {
                var contadores = await _service.ObtenerContadoresMP(año, mes);
                return Ok(contadores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener los contadores de mantenimientos preventivos",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Guarda un snapshot de contadores al cierre de un mes
        /// POST api/KPIMantenimientoPreventivo/historico?año=2025&mes=5
        /// </summary>
        [HttpPost("historico")]
        public async Task<IActionResult> GuardarHistorico([FromQuery] int año, [FromQuery] int mes)
        {
            try
            {
                await _service.GuardarContadoresMPHistorico(año, mes);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al guardar el snapshot histórico",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Devuelve el histórico de snapshots guardados
        /// GET api/KPIMantenimientoPreventivo/historico?anio=2025&mes=5
        /// </summary>
        [HttpGet("historico")]
        public async Task<IActionResult> GetHistorico([FromQuery] int? año = null, [FromQuery] int? mes = null)
        {
            try
            {
                var lista = await _service.ObtenerHistoricoMP(año, mes);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener el histórico",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Devuelve los contadores programados de MP para un año: Total, Anteriores (≤ semana actual) y Futuros (> semana actual).
        /// GET api/KPIMantenimientoPreventivo/programados?anio=2025
        /// </summary>
        [HttpGet("programados")]
        public async Task<IActionResult> GetContadoresProgramados([FromQuery] int? anio = null)
        {
            try
            {
                int añoUso = anio ?? DateTime.Now.Year;
                var cont = await _service.ObtenerContadoresProgramadosMP(añoUso);
                return Ok(cont);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener los contadores programados de mantenimientos preventivos",
                    error = ex.Message
                });
            }
        }

    }
}
