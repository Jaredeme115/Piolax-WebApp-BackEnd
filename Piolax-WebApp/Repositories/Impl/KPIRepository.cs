/*using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<KpisDetalle>> ConsultarMTTA(int? idArea = null, int? idMaquina = null,
    int? año = null, int? mes = null, int? semana = null, int? diaSemana = null)
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

            if (año.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.fechaCalculo.Year == año.Value);
            }

            if (mes.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.fechaCalculo.Month == mes.Value);
            }

            // Ejecuta la consulta hasta aquí
            var results = await query.ToListAsync();
            
            // Aplica filtros de semana y día de la semana en memoria
            if (semana.HasValue)
            {
                results = results.Where(kd => GetWeekOfYear(kd.KpisMantenimiento.fechaCalculo) == semana.Value).ToList();
            }

            if (diaSemana.HasValue)
            {
                results = results.Where(kd => (int)kd.KpisMantenimiento.fechaCalculo.DayOfWeek == (diaSemana.Value % 7)).ToList();
            }

            return results;
        }


        public async Task<IEnumerable<KpisDetalle>> ConsultarMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null,
            int? año = null, int? mes = null, int? semana = null, int? diaSemana = null)
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

            if (año.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.fechaCalculo.Year == año.Value);
            }

            if (mes.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.fechaCalculo.Month == mes.Value);
            }

            // Ejecuta la consulta hasta aquí
            var results = await query.ToListAsync();


            // Aplica filtros de semana y día de la semana en memoria
            if (semana.HasValue)
            {
                results = results.Where(kd => GetWeekOfYear(kd.KpisMantenimiento.fechaCalculo) == semana.Value).ToList();
            }

            if (diaSemana.HasValue)
            {
                results = results.Where(kd => (int)kd.KpisMantenimiento.fechaCalculo.DayOfWeek == (diaSemana.Value % 7)).ToList();
            }

            return results;
        }

        public async Task<IEnumerable<KpisDetalle>> ConsultarMTBF(int? idArea = null, int? idMaquina = null,
            int? año = null, int? mes = null, int? semana = null, int? diaSemana = null)
        {
            var query = _context.KpisDetalle
                .Include(kd => kd.KpisMantenimiento)
                .Where(kd => kd.kpiNombre == "MTBF");

            if (idArea.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.idArea == idArea.Value);
            }

            if (idMaquina.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.idMaquina == idMaquina.Value);
            }

            if (año.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.fechaCalculo.Year == año.Value);
            }

            if (mes.HasValue)
            {
                query = query.Where(kd => kd.KpisMantenimiento.fechaCalculo.Month == mes.Value);
            }

            // Ejecuta la consulta hasta aquí
            var results = await query.ToListAsync();

            // Aplica filtros de semana y día de la semana en memoria
            if (semana.HasValue)
            {
                results = results.Where(kd => GetWeekOfYear(kd.KpisMantenimiento.fechaCalculo) == semana.Value).ToList();
            }

            if (diaSemana.HasValue)
            {
                results = results.Where(kd => (int)kd.KpisMantenimiento.fechaCalculo.DayOfWeek == (diaSemana.Value % 7)).ToList();
            }

            return results;
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

            // Ejecuta la consulta hasta aquí
            var results = await query.ToListAsync();

            // Aplica filtros de semana y día de la semana en memoria
            if (semana.HasValue)
            {
                results = results.Where(km => GetWeekOfYear(km.fechaCalculo) == semana.Value).ToList();
            }

            if (diaSemana.HasValue)
            {
                results = results.Where(km => (int)km.fechaCalculo.DayOfWeek == (diaSemana.Value % 7)).ToList();
            }

            return results;
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
}*/

using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using SkiaSharp;

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

        public async Task<IEnumerable<KpisDetalle>> ConsultarMTTA(int? idArea = null, int? idMaquina = null, int? anio = null, int? mes = null)
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


            return resultados;
        }

        public async Task<IEnumerable<KpisDetalle>> ConsultarMTTR(int? idArea = null, int? idMaquina = null, int? idEmpleado = null, int? anio = null, int? mes = null)
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

        public async Task<IEnumerable<KpisMantenimiento>> ConsultarTotalDowntime(
        int? idArea = null,
        int? idMaquina = null,
        int? anio = null,
        int? mes = null,
        int? semana = null,
        int? diaSemana = null)
        {
            // 1) Filtros traducibles a SQL
            var query = _context.KpisMantenimiento
                .Include(km => km.KpisDetalle)
                .AsQueryable();

            if (idArea.HasValue)
                query = query.Where(km => km.idArea == idArea.Value);

            if (idMaquina.HasValue)
                query = query.Where(km => km.idMaquina == idMaquina.Value);

            if (anio.HasValue)
                query = query.Where(km => km.fechaCalculo.Year == anio.Value);

            if (mes.HasValue)
                query = query.Where(km => km.fechaCalculo.Month == mes.Value);

            // 2) Materializar la lista
            var lista = await query.ToListAsync();

            // 3) Filtrar por semana en memoria
            if (semana.HasValue)
            {
                lista = lista
                    .Where(km => System.Globalization.ISOWeek.GetWeekOfYear(km.fechaCalculo) == semana.Value)
                    .ToList();
            }

            // 4) Filtrar por día de la semana en memoria
            if (diaSemana.HasValue)
            {
                lista = lista
                    .Where(km => (int)km.fechaCalculo.DayOfWeek == diaSemana.Value)
                    .ToList();
            }

            return lista;
        }

        public async Task<List<MTBFPorAreaMesDTO>> ConsultarMTBFPorAreaMes(int anio)
        {
            return await _context.KpisDetalle
            .Include(kd => kd.KpisMantenimiento)
                .ThenInclude(km => km.Area)
            .Where(kd =>
                kd.kpiNombre == "MTBF" &&
                kd.KpisMantenimiento.fechaCalculo.Year == anio)
            .GroupBy(kd => new
            {               kd.KpisMantenimiento.idArea,
                nombreArea = kd.KpisMantenimiento.Area.nombreArea,
                mes = kd.KpisMantenimiento.fechaCalculo.Month
            })
            .Select(g => new MTBFPorAreaMesDTO
            {
                idArea      = g.Key.idArea,
                nombreArea  = g.Key.nombreArea,
                mes         = g.Key.mes,
                valorHoras  = g.Average(x => x.kpiValor) / 60f
            })
            .ToListAsync();
        }

        // Método para Objetivos del MTBF

        /*public async Task<KpiObjetivos> GuardarObjetivoAsync(KpiObjetivos objetivo)
        {
            // Si ya existe, actualiza, si no, inserta
            var existente = await _context.KpiObjetivos
                .FirstOrDefaultAsync(o => o.idArea == objetivo.idArea && o.idArea == objetivo.anio && o.mes == objetivo.mes);
            if (existente != null)
            {
                existente.valorHoras = objetivo.valorHoras;
                return existente;
            }
            _context.KpiObjetivos.Add(objetivo);
            await _context.SaveChangesAsync();
            return objetivo;
        }*/

        public async Task<KpiObjetivos> GuardarObjetivoAsync(KpiObjetivos objetivo)
        {
            // Si ya existe (mismo área, año y mes), actualiza; si no, inserta
            var existente = await _context.KpiObjetivos
                .FirstOrDefaultAsync(o =>
                    o.idArea == objetivo.idArea
                 && o.anio == objetivo.anio    // <-- aquí faltaba anio
                 && o.mes == objetivo.mes);
            if (existente != null)
            {
                existente.valorHoras = objetivo.valorHoras;
            }
            else
            {
                _context.KpiObjetivos.Add(objetivo);
            }
            await _context.SaveChangesAsync();
            return existente ?? objetivo;
        }


        public Task<KpiObjetivos> ObtenerObjetivoAsync(int idArea, int anio, int mes) =>
            _context.KpiObjetivos
              .FirstOrDefaultAsync(o => o.idArea == idArea && o.anio == anio && o.mes == mes);

        public Task<List<KpiObjetivos>> ObtenerObjetivosPorAnioAsync(int anio) =>
            _context.KpiObjetivos
              .Where(o => o.anio == anio)
              .ToListAsync();


        //------------------------------------------------------------------------------------------------------//

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
