using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class KPIRepository(AppDbContext context) : IKPIRepository
    {
        private readonly AppDbContext _context = context;

        // KPIs para Mantenimiento Correctivo
        public async Task GuardarKPIMantenimiento(KpisMantenimiento kpiMantenimiento)
        {
            _context.KpisMantenimiento.Add(kpiMantenimiento);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarKPIDetalles(int idKPIMantenimiento, List<KpisDetalle> kpiDetalles)
        {
            foreach (var detalle in kpiDetalles)
            {
                detalle.idKPIMantenimiento = idKPIMantenimiento;
                _context.KpisDetalle.Add(detalle);
            }
            await _context.SaveChangesAsync();
        }

        // KPIs para Mantenimiento Preventivo

        public async Task GuardarKPIPreventivo(KpisMP kpisMP)
        {
            _context.KpisMP.Add(kpisMP);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarKPIDetallesMP(int idKPIMP, List<KpisMPDetalle> kpiDetalles)
        {
            foreach (var detalle in kpiDetalles)
            {
                detalle.idKPIMP = idKPIMP;
                _context.KpisMPDetalle.Add(detalle);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<KpisDetalle>> ConsultarMTTA(int? idArea = null, int? idMaquina = null)
        {
            var query = _context.KpisDetalle
                .Include(kd => kd.KpisMantenimiento)
                .Where(kd => kd.kpiNombre == "MTTA");

            if (idArea.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.idArea == idArea.Value);
            }

            if (idMaquina.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.idMaquina == idMaquina.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<KpisDetalle>> ConsultarMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null)
        {
            var query = _context.KpisDetalle
                .Include(kd => kd.KpisMantenimiento)
                .Where(kd => kd.kpiNombre == "MTTR");

            if (idArea.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.idArea == idArea.Value);
            }

            if (idMaquina.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.idMaquina == idMaquina.Value);
            }

            if (idEmpleado.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.idEmpleado == idEmpleado.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<KpisDetalle>> ConsultarMTBF(int? idArea = null)
        {
            var query = _context.KpisDetalle
                .Include(kd => kd.KpisMantenimiento)
                .Where(kd => kd.kpiNombre == "MTBF");

            if (idArea.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.idArea == idArea.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<KpisMantenimiento>> ConsultarTotalDowntime(int? idArea = null, int? idMaquina = null, int? año = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var query = _context.KpisMantenimiento
                .Include(km => km.KpisDetalle)
                .AsQueryable();

            if (idArea.HasValue)
            {
                query = query.Where(km => km.idArea == idArea.Value);
            }

            if (idMaquina.HasValue)
            {
                query = query.Where(km => km.idMaquina == idMaquina.Value);
            }

            if (año.HasValue)
            {
                query = query.Where(km => km.fechaCalculo.Year == año.Value);
            }

            if (mes.HasValue)
            {
                query = query.Where(km => km.fechaCalculo.Month == mes.Value);
            }

            if (semana.HasValue)
            {
                // Aplicar el filtro por semana del año
                query = query.Where(km => GetWeekOfYear(km.fechaCalculo) == semana.Value);
            }

            if (diaSemana.HasValue)
            {
                // Aplicar el filtro por día de la semana (1=Lunes, 7=Domingo)
                query = query.Where(km => (int)km.fechaCalculo.DayOfWeek == (diaSemana.Value % 7));
            }

            return await query.ToListAsync();
        }

        private int GetWeekOfYear(DateTime date)
        {
            var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            return calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

    }
}
