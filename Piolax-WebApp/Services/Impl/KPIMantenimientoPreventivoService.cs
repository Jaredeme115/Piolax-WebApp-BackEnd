using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class KPIMantenimientoPreventivoService (IKPIRepository repository, IMantenimientoPreventivoRepository mantenimientoPreventivoRepository) : IKPIMantenimientoPreventivoService
    {
        private readonly IKPIRepository _kpiRepository = repository;
        private readonly IMantenimientoPreventivoRepository _mantenimientoPreventivoRepository = mantenimientoPreventivoRepository;

        public async Task CalcularYGuardarKPIs(DateTime inicio, DateTime fin)
        {
            // 1️⃣ Obtener los mantenimientos del período
            var mps = await _mantenimientoPreventivoRepository.ConsultarMantenimientosPorPeriodo(inicio, fin);

            int totalProgramados = mps.Count();
            int totalEjecutados = mps.Count(mp => mp.fechaEjecucion != null);

            // 2️⃣ Calcular Cumplimiento (Siempre 100% si se ejecuta en algún momento)
            float cumplimiento = totalProgramados > 0 ? (totalEjecutados / (float)totalProgramados * 100) : 100;

            // 3️⃣ Calcular Efectividad (MP ejecutados en la semana programada)
            int ejecutadosEnTiempo = mps
                .Where(mp => mp.fechaEjecucion != null)
                .Count(mp => GetWeekOfYear(mp.fechaEjecucion.Value) == mp.semanaPreventivo);
            float efectividad = totalEjecutados > 0 ? (ejecutadosEnTiempo / (float)totalEjecutados * 100) : 0;

            // 4️⃣ Guardar el KPI en la tabla `KpisMP`
            var kpi = new KpisMP
            {
                fechaCalculo = DateTime.Now
            };
            await _kpiRepository.GuardarKPIPreventivo(kpi);

            // 5️⃣ Crear y guardar los detalles de KPI
            var detalles = new List<KpisMPDetalle>
        {
            new KpisMPDetalle { kpiMPNombre = "Cumplimiento", kpiMPValor = cumplimiento },
            new KpisMPDetalle { kpiMPNombre = "Efectividad", kpiMPValor = efectividad }
        };

            await _kpiRepository.GuardarKPIDetallesMP(kpi.idKPIMP, detalles);
        }

        public async Task<KPIResponseDTO> ObtenerCumplimiento(int? anio = null, int? mes = null)
        {
            var kpiDetalles = await _kpiRepository.ConsultarKPIsDetallePreventivo("Cumplimiento", anio, mes);

            if (!kpiDetalles.Any())
                return new KPIResponseDTO { Nombre = "Cumplimiento", Valor = 0, UnidadMedida = "%" };

            // Calcular el promedio de los valores de Cumplimiento
            float valorPromedio = kpiDetalles.Average(k => k.kpiMPValor);

            return new KPIResponseDTO
            {
                Nombre = "Cumplimiento",
                Valor = valorPromedio,
                UnidadMedida = "%"
            };
        }

        public async Task<KPIResponseDTO> ObtenerEfectividad(int? anio = null, int? mes = null)
        {
            var kpiDetalles = await _kpiRepository.ConsultarKPIsDetallePreventivo("Efectividad", anio, mes);

            if (!kpiDetalles.Any())
                return new KPIResponseDTO { Nombre = "Efectividad", Valor = 0, UnidadMedida = "%" };

            // Calcular el promedio de los valores de Efectividad
            float valorPromedio = kpiDetalles.Average(k => k.kpiMPValor);

            return new KPIResponseDTO
            {
                Nombre = "Efectividad",
                Valor = valorPromedio,
                UnidadMedida = "%"
            };
        }

        public async Task<IEnumerable<KPIResponseDTO>> ObtenerResumenKPIsPreventivo(int? anio = null, int? mes = null)
        {
            var resultados = new List<KPIResponseDTO>();

            // Obtener cumplimiento
            var cumplimiento = await ObtenerCumplimiento(anio, mes);
            resultados.Add(cumplimiento);

            // Obtener efectividad
            var efectividad = await ObtenerEfectividad(anio, mes);
            resultados.Add(efectividad);

            return resultados;
        }

        public async Task<ContadoresMPDTO> ObtenerContadoresMP(int? anio = null, int? mes = null)
        {
            List<MantenimientoPreventivo> mps;

            if (anio.HasValue && mes.HasValue)
            {
                // Usa el nuevo método del repositorio que ya tiene la lógica por semana y año
                mps = (await _mantenimientoPreventivoRepository.ConsultarPorAnioYMes(anio.Value, mes.Value)).ToList();
            }
            else
            {
                // Si no hay año o mes, traer todos
                mps = (await _mantenimientoPreventivoRepository.ConsultarTodosMPs()).ToList();

                // Si solo hay año, filtrar por él
                if (anio.HasValue)
                {
                    mps = mps.Where(mp => mp.anioPreventivo == anio.Value).ToList();
                }
            }

            return new ContadoresMPDTO
            {
                TotalMP = mps.Count(),
                Pendientes = mps.Count(mp => mp.idEstatusPreventivo == 1),
                NoRealizados = mps.Count(mp => mp.idEstatusPreventivo == 2),
                Realizados = mps.Count(mp => mp.idEstatusPreventivo == 3),
                Reprogramados = mps.Count(mp => mp.idEstatusPreventivo == 4),
                EnProceso = mps.Count(mp => mp.idEstatusPreventivo == 5)
            };
        }

        private int GetWeekOfYear(DateTime date)
        {
            var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            return calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }
}
  