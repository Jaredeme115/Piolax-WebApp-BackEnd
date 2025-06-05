using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Hubs;
using Piolax_WebApp.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Piolax_WebApp.Controllers
{
    public class KPIDashboardController(IKPIDashboardService service, IHubContext<NotificationHub> hubContext) : BaseApiController
    {
        private readonly IKPIDashboardService _service = service;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;

        #region MTTA Endpoints
        /// <summary>
        /// Obtiene el indicador MTTA (Mean Time To Acknowledge) aplicando los filtros especificados
        /// </summary>
        [HttpGet("mtta")]
        public async Task<IActionResult> GetMTTA(
            int? idArea = null,
            int? idMaquina = null,
            int? anio = null,
            int? mes = null)
        {
            var result = await _service.ObtenerMTTA(idArea, idMaquina, anio, mes);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el MTTA segmentado por períodos de tiempo para visualización en gráficos
        /// </summary>
        [HttpGet("mtta/segmentado")]
        public async Task<ActionResult<List<KpiSegmentadoDTO>>> GetMTTASegmentado(
            int? idArea = null,
            int? idMaquina = null,
            int? anio = null,
            int? mes = null)
        {
            var resultado = await _service.ObtenerMTTASegmentado(idArea, idMaquina, anio, mes);
            return Ok(resultado);
        }
        #endregion

        #region MTTR Endpoints
        /// <summary>
        /// Obtiene el indicador MTTR (Mean Time To Repair) aplicando los filtros especificados
        /// </summary>
        [HttpGet("mttr")]
        public async Task<IActionResult> GetMTTR(
            int? idArea = null,
            int? idMaquina = null,
            int? idEmpleado = null,
            int? anio = null,
            int? mes = null)
        {
            var result = await _service.ObtenerMTTR(idArea, idMaquina, idEmpleado, anio, mes);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el MTTR segmentado por períodos de tiempo para visualización en gráficos
        /// </summary>
        [HttpGet("mttr/segmentado")]
        public async Task<ActionResult<List<KpiSegmentadoDTO>>> GetMTTRSegmentado(
            int? idArea = null,
            int? idMaquina = null,
            int? idEmpleado = null,
            int? anio = null,
            int? mes = null)
        {
            var resultado = await _service.ObtenerMTTRSegmentado(idArea, idMaquina, idEmpleado, anio, mes);
            return Ok(resultado);
        }
        #endregion

        #region MTBF Endpoints
        /// <summary>
        /// Obtiene el indicador MTBF (Mean Time Between Failures) aplicando los filtros especificados
        /// </summary>
        [HttpGet("mtbf")]
        public async Task<IActionResult> GetMTBF(
            int? idArea = null,
            int? objetivo = null)
        {
            var result = await _service.ObtenerMTBF(idArea);

            // Si hay un objetivo definido, agregamos información de cumplimiento
            if (objetivo.HasValue && objetivo.Value > 0)
            {
                var cumplimiento = (result.Valor / objetivo.Value) * 100;

                return Ok(new
                {
                    KPI = result,
                    Objetivo = objetivo.Value,
                    PorcentajeCumplimiento = cumplimiento,
                    Cumple = result.Valor >= objetivo.Value
                });
            }

            return Ok(result);
        }

        /// <summary>
        /// Obtiene el MTBF segmentado por mes (en HORAS) para un área y año dados.
        /// </summary>
        [HttpGet("mtbf/segmentado")]
        public async Task<ActionResult<List<KpiSegmentadoDTO>>> GetMTBFSegmentadoPorMes(
            int? idArea = null,
            int? anio = null)
        {
            if (!idArea.HasValue)
                return BadRequest("Se debe especificar idArea.");

            int areaToUse = idArea.Value;
            int yearToUse = anio ?? DateTime.Now.Year;

            // Llamamos al servicio que recorre los 12 meses
            var mtbfSegmentado = await _service.ObtenerMTBFPorAreaMes(areaToUse, yearToUse);

            return Ok(mtbfSegmentado);
        }

        // Metodos para KPI Objetivos
        [HttpPost("mtbf/objetivo")]
        public async Task<IActionResult> PostObjetivo([FromBody] KpiObjetivosDTO dto)
        {
            await _service.GuardarObjetivo(dto.idArea, dto.anio, dto.mes, dto.valorHoras);
            return Ok();
        }

        [HttpGet("mtbf/objetivos/{anio}")]
        public async Task<IActionResult> GetObjetivos(int anio)
        {
            var datos = await _service.ObtenerObjetivosPorAnio(anio);
            return Ok(datos);
        }

        // 2) Para objetivos
        [HttpGet("mtbf/objetivos/series/{anio}")]
        public async Task<IActionResult> GetObjetivosSeries(int anio)
        {
            var data = await _service.ObtenerObjetivosSeriesAsync(anio);     // devuelve List<KpiObjetivosSeriesDTO>
            return Ok(data);
        }


        #endregion

        [HttpGet("totaldowntime")]
        public async Task<IActionResult> GetTotalDowntime(
            int? idArea = null, int? idMaquina = null,
            int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var result = await _service.ObtenerTotalDowntime(
                idArea, idMaquina, anio, mes, semana, diaSemana);
            return Ok(result);
        }

        [HttpGet("totaldowntime/segmentado")]
        public async Task<ActionResult<List<KpiSegmentadoDTO>>> GetTotalDowntimeSegmentado(
        int? idArea = null,
        int? idMaquina = null,
        int? anio = null,
        int? mes = null,
        int? semana = null,
        int? diaSemana = null)
        {
            var resultado = await _service.ObtenerTotalDowntimeSegmentado(
                idArea, idMaquina, anio, mes, semana, diaSemana);
            return Ok(resultado);
        }


        #region Dashboard y Resúmenes
        /// <summary>
        /// Obtiene un resumen de todos los KPIs con filtros independientes para cada uno
        /// </summary>
        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumenKPIs(
            // Parámetros MTTA
            int? idAreaMTTA = null, int? idMaquinaMTTA = null,
            int? anioMTTA = null, int? mesMTTA = null,
            // Parámetros MTTR
            int? idAreaMTTR = null, int? idMaquinaMTTR = null, int? idEmpleadoMTTR = null,
            int? anioMTTR = null, int? mesMTTR = null,
            // Parámetros MTBF
            int? idAreaMTBF = null, int? objetivoMTBF = null,
            // Parámetros TotalDowntime
            int? idAreaDowntime = null, int? idMaquinaDowntime = null,
            int? anioDowntime = null, int? mesDowntime = null, int? semanaDowntime = null, int? diaSemanaDowntime = null)
        {
            var result = await _service.ObtenerResumenKPIs(
                idAreaMTTA, idMaquinaMTTA, anioMTTA, mesMTTA,
                idAreaMTTR, idMaquinaMTTR, idEmpleadoMTTR, anioMTTR, mesMTTR,
                idAreaMTBF,
                idAreaDowntime, idMaquinaDowntime,
                anioDowntime, mesDowntime, semanaDowntime, diaSemanaDowntime);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene los datos para el dashboard en tiempo real con los KPIs básicos
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(
            int? idArea = null,
            int? idMaquina = null,
            int? idEmpleado = null,
            int? objetivoMTBF = null)
        {
            // Obtener datos básicos para el dashboard
            var mtta = await _service.ObtenerMTTA(idArea, idMaquina);
            var mttr = await _service.ObtenerMTTR(idArea, idMaquina, idEmpleado);
            var mtbf = await _service.ObtenerMTBF(idArea);
            var totalDowntime = await _service.ObtenerTotalDowntime(idArea, idMaquina);

            // Calcular cumplimiento del MTBF si hay objetivo
            double porcentajeCumplimientoMTBF = 0;
            bool cumpleMTBF = false;

            if (objetivoMTBF.HasValue && objetivoMTBF.Value > 0)
            {
                porcentajeCumplimientoMTBF = (mtbf.Valor / objetivoMTBF.Value) * 100;
                cumpleMTBF = mtbf.Valor >= objetivoMTBF.Value;
            }

            // Crear objeto para el dashboard
            var dashboard = new
            {
                FechaActualizacion = DateTime.Now,
                KPIs = new List<KPIResponseDTO> { mtta, mttr, mtbf, totalDowntime },
                ResumenMTBF = new
                {
                    Valor = mtbf.Valor,
                    Objetivo = objetivoMTBF,
                    PorcentajeCumplimiento = porcentajeCumplimientoMTBF,
                    Cumple = cumpleMTBF
                },
                // Agregar tendencias (a implementar con datos históricos)
                Tendencias = new
                {
                    MTTA = new { Actual = mtta.Valor, Anterior = mtta.Valor * 1.1f, Variacion = -10 },
                    MTTR = new { Actual = mttr.Valor, Anterior = mttr.Valor * 0.9f, Variacion = 10 },
                    MTBF = new { Actual = mtbf.Valor, Anterior = mtbf.Valor * 0.95f, Variacion = 5 }
                }
            };

            return Ok(dashboard);
        }
        #endregion

        #region Real-time Updates
        /// <summary>
        /// Inicia el stream de datos en tiempo real para los KPIs
        /// </summary>
        [HttpPost("realtime/start")]
        public async Task<IActionResult> StartRealtimeKPIUpdates(
            int? idArea = null,
            int? idMaquina = null,
            int? idEmpleado = null)
        {
            // El grupo de SignalR será específico para esta combinación de filtros
            string groupName = $"KPI_{idArea ?? 0}_{idMaquina ?? 0}_{idEmpleado ?? 0}";

            // Registrar que hay clientes interesados en estos KPIs (para uso futuro)
            // Esto podría guardarse en memoria o en una base de datos

            // Enviar confirmación inicial
            await _hubContext.Clients.Group(groupName).SendAsync(
                "KPIStreamStarted",
                new { Message = "Flujo de KPIs iniciado", Timestamp = DateTime.Now }
            );

            return Ok(new
            {
                Message = "Flujo de KPIs iniciado correctamente",
                GroupName = groupName,
                Instructions = "Conéctese al hub de SignalR y suscríbase al grupo especificado para recibir actualizaciones"
            });
        }

        /// <summary>
        /// Envía actualizaciones manuales de KPIs a través de SignalR
        /// </summary>
        [HttpPost("realtime/update")]
        public async Task<IActionResult> TriggerManualKPIUpdate(
            int? idArea = null,
            int? idMaquina = null,
            int? idEmpleado = null)
        {
            // Obtener datos actualizados
            var mtta = await _service.ObtenerMTTA(idArea, idMaquina);
            var mttr = await _service.ObtenerMTTR(idArea, idMaquina, idEmpleado);
            var mtbf = await _service.ObtenerMTBF(idArea);
            var totalDowntime = await _service.ObtenerTotalDowntime(idArea, idMaquina);

            // Crear payload con los KPIs actualizados
            var kpiUpdate = new
            {
                Timestamp = DateTime.Now,
                MTTA = mtta,
                MTTR = mttr,
                MTBF = mtbf,
                TotalDowntime = totalDowntime
            };

            // El grupo de SignalR será específico para esta combinación de filtros
            string groupName = $"KPI_{idArea ?? 0}_{idMaquina ?? 0}_{idEmpleado ?? 0}";

            // Enviar actualización a través de SignalR
            await _hubContext.Clients.Group(groupName).SendAsync("KPIUpdate", kpiUpdate);

            // También enviar a grupos más generales
            if (idArea.HasValue)
            {
                await _hubContext.Clients.Group($"Area_{idArea}").SendAsync("KPIUpdate", kpiUpdate);
            }

            // Notificar a todos los que monitorean mantenimiento
            await _hubContext.Clients.Group("Mantenimiento").SendAsync("KPIUpdate", kpiUpdate);

            return Ok(new
            {
                Message = "Actualización de KPIs enviada correctamente",
                KPIs = kpiUpdate
            });
        }
        #endregion

        #region Internal KPIs Service Status
        /// <summary>
        /// Endpoint para verificar el estado del servicio de KPIs
        /// </summary>
        [HttpGet("status")]
        public IActionResult GetServiceStatus()
        {
            return Ok(new
            {
                Status = "Operational",
                LastUpdate = DateTime.Now,
                Version = "1.0.0"
            });
        }
        #endregion
    }
}
