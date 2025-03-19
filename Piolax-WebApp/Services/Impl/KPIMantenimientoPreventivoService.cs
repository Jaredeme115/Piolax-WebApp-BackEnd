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

        private int GetWeekOfYear(DateTime date)
        {
            var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            return calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }
}
