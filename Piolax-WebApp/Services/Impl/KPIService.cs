using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Repositories;
using System.Globalization;

namespace Piolax_WebApp.Services.Impl
{
    public class KPIService : IKPIService
    {
        private readonly AppDbContext _context;

        public KPIService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<KpiDashboardDTO>> ObtenerKPIs(KPIFiltroDTO filtros)
        {
            var query = _context.KpisMantenimiento
       .Include(k => k.KpisDetalle) // 👈 asegúrate que tienes esta navegación
       .AsQueryable();

            if (filtros.idArea.HasValue)
                query = query.Where(k => k.idArea == filtros.idArea);
            if (filtros.idMaquina.HasValue)
                query = query.Where(k => k.idMaquina == filtros.idMaquina);
            if (filtros.idEmpleado.HasValue)
                query = query.Where(k => k.idEmpleado == filtros.idEmpleado);
            if (filtros.anio.HasValue)
                query = query.Where(k => k.fechaCalculo.Year == filtros.anio);
            if (filtros.mes.HasValue)
                query = query.Where(k => k.fechaCalculo.Month == filtros.mes);

            if (filtros.semana.HasValue && filtros.anio.HasValue)
            {
                var fechaInicio = ISOWeekToDate(filtros.anio.Value, filtros.semana.Value);
                var fechaFin = fechaInicio.AddDays(6);

                query = query.Where(k => k.fechaCalculo >= fechaInicio && k.fechaCalculo <= fechaFin);
            }

            if (!string.IsNullOrEmpty(filtros.dia))
                query = query.Where(k => k.fechaCalculo.Date == DateTime.Parse(filtros.dia).Date);

            var lista = await query.Select(k => new KpiDashboardDTO
            {
                fechaCalculo = k.fechaCalculo,
                mtta = k.KpisDetalle.FirstOrDefault(d => d.kpiNombre == "MTTA")!.kpiValor,
                mttr = k.KpisDetalle.FirstOrDefault(d => d.kpiNombre == "MTTR")!.kpiValor,
                totalDowntime = k.KpisDetalle.FirstOrDefault(d => d.kpiNombre == "Downtime")!.kpiValor
            }).ToListAsync();

            return lista;
        }
        public static DateTime ISOWeekToDate(int year, int week)
        {
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            var firstThursday = jan1.AddDays(daysOffset);
            var firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = week;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

    }
}
