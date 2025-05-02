using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<IEnumerable<KpisDetalle>> ConsultarMTTA(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null)
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

            if (anio.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.fechaCalculo.Year == anio);
            }
            if (mes.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.fechaCalculo.Month == mes);
            }

            var resultados = await query.ToListAsync();

            // 🔥 Aquí filtramos por semana
            if (semana.HasValue)
            {
                resultados = resultados
                    .Where(kd => System.Globalization.ISOWeek.GetWeekOfYear(kd.KpisMantenimiento.fechaCalculo) == semana.Value)
                    .ToList();
            }

            // 🔥 Aquí filtramos por día de la semana
            if (diaSemana.HasValue)
            {
                resultados = resultados
                    .Where(kd => (int)kd.KpisMantenimiento.fechaCalculo.DayOfWeek == diaSemana.Value)
                    .ToList();
            }

            return resultados;
        }

        public async Task<IEnumerable<KpisDetalle>> ConsultarMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null, int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null)
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
            if (anio.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.fechaCalculo.Year == anio);
            }
            if (mes.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.fechaCalculo.Month == mes);
            }

            var resultados = await query.ToListAsync();

            // 🔥 Aquí filtramos por semana
            if (semana.HasValue)
            {
                resultados = resultados
                    .Where(kd => System.Globalization.ISOWeek.GetWeekOfYear(kd.KpisMantenimiento.fechaCalculo) == semana.Value)
                    .ToList();
            }

            // 🔥 Aquí filtramos por día de la semana
            if (diaSemana.HasValue)
            {
                resultados = resultados
                    .Where(kd => (int)kd.KpisMantenimiento.fechaCalculo.DayOfWeek == diaSemana.Value)
                    .ToList();
            }

            return resultados;
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

        public async Task<IEnumerable<KpisMantenimiento>> ConsultarTotalDowntime(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null, int? semana = null, int? diaSemana = null)
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

            if (anio.HasValue)
            {
                query = query.Where(km => km.fechaCalculo.Year == anio.Value);
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

        public async Task<IEnumerable<KpisMP>> ConsultarKPIsPreventivo(int? año = null, int? mes = null)
        {
            // Usamos Include para incluir la relación KpisMPDetalle
            var query = _context.KpisMP
                .Include(kp => kp.KpisMPDetalle)
                .AsQueryable();

            if (año.HasValue)
            {
                query = query.Where(kp => kp.fechaCalculo.Year == año.Value);
            }

            if (mes.HasValue)
            {
                query = query.Where(kp => kp.fechaCalculo.Month == mes.Value);
            }

            // Ejecuta la consulta hasta aquí
            var results = await query.ToListAsync();

            return results;
        }

        public async Task<IEnumerable<KpisMPDetalle>> ConsultarKPIsDetallePreventivo(string nombreKPI = null, int? año = null, int? mes = null)
        {
            // Aquí está el problema: debes importar el tipo correcto
            // Primero obtenemos la consulta base sin el Include
            var query = _context.KpisMPDetalle.AsQueryable();

            // Añadimos el Include de forma correcta
            query = query.Include(kd => kd.KpisMP);

            if (!string.IsNullOrEmpty(nombreKPI))
            {
                query = query.Where(kd => kd.kpiMPNombre == nombreKPI);
            }

            if (año.HasValue)
            {
                query = query.Where(kd => kd.KpisMP.fechaCalculo.Year == año.Value);
            }

            if (mes.HasValue)
            {
                query = query.Where(kd => kd.KpisMP.fechaCalculo.Month == mes.Value);
            }

            // Ejecuta la consulta hasta aquí
            var results = await query.ToListAsync();

            return results;
        }

        private int GetWeekOfYear(DateTime date)
        {
            var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            return calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }





    }
}
