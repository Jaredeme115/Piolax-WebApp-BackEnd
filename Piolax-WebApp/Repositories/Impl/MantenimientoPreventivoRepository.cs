using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class MantenimientoPreventivoRepository (AppDbContext context) : IMantenimientoPreventivoRepository
    {
        private readonly AppDbContext _context = context;

        // Método para crear un nuevo mantenimiento preventivo
        public async Task<MantenimientoPreventivo> CrearMantenimientoPreventivo(MantenimientoPreventivo mantenimientoPreventivo)
        {
            _context.MantenimientoPreventivo.Add(mantenimientoPreventivo);
            await _context.SaveChangesAsync();
            return mantenimientoPreventivo;

        }


        // Método para consultar un mantenimiento preventivo con detalles
        public async Task<MantenimientoPreventivo?> ConsultarMP(int idMP)
        {
            return await _context.MantenimientoPreventivo
                .Include(mp => mp.Area)
                .Include(mp => mp.Maquina)
                .Include(mp => mp.FrecuenciaMP)
                .Include(mp => mp.EstatusPreventivo)
                .FirstOrDefaultAsync(mp => mp.idMP == idMP);
        }


        // Método para modificar un mantenimiento preventivo
        // Método para modificar un mantenimiento preventivo
        public async Task<MantenimientoPreventivo?> Modificar(int idMP, MantenimientoPreventivo mantenimientoPreventivo)
        {
            var mantenimientoExistente = await _context.MantenimientoPreventivo.FindAsync(idMP);

            if (mantenimientoExistente == null) return null; // Retorna null si no existe

            mantenimientoExistente.semanaPreventivo = mantenimientoPreventivo.semanaPreventivo;
            mantenimientoExistente.semanaOriginalMP = mantenimientoPreventivo.semanaOriginalMP;
            mantenimientoExistente.anioPreventivo = mantenimientoPreventivo.anioPreventivo;

            mantenimientoExistente.idFrecuenciaPreventivo = mantenimientoPreventivo.idFrecuenciaPreventivo;
            mantenimientoExistente.idEstatusPreventivo = mantenimientoPreventivo.idEstatusPreventivo;
            mantenimientoExistente.activo = mantenimientoPreventivo.activo;
            mantenimientoExistente.ultimaEjecucion = mantenimientoPreventivo.ultimaEjecucion;
            mantenimientoExistente.proximaEjecucion = mantenimientoPreventivo.proximaEjecucion;

            await _context.SaveChangesAsync();
            return mantenimientoExistente; // Retorna el mantenimiento actualizado
        }



        // Método para eliminar un mantenimiento preventivo
        public async Task<bool> Eliminar(int idMP)
        {
            var mantenimientoPreventivo = await _context.MantenimientoPreventivo.FindAsync(idMP);
            if (mantenimientoPreventivo == null) return false;

            _context.MantenimientoPreventivo.Remove(mantenimientoPreventivo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MantenimientoPreventivo>> ConsultarTodosMPs()
        {
            return await _context.MantenimientoPreventivo
                .Include(mp => mp.Area)
                .Include(mp => mp.Maquina)
                .Include(mp => mp.FrecuenciaMP)
                .Include(mp => mp.EstatusPreventivo)
                .Include(mp => mp.Empleado)
                .OrderBy(mp => mp.anioPreventivo).ThenBy(mp => mp.semanaPreventivo) // ✅ orden por año-semana
                .ToListAsync();
        }

        public async Task<IEnumerable<MantenimientoPreventivo>> MostrarMPsAsignados(int idEmpleado)
        {
            return await _context.MantenimientoPreventivo
             .Include(mp => mp.Area)
             .Include(mp => mp.Maquina)
             .Include(mp => mp.FrecuenciaMP)
             .Include(mp => mp.EstatusPreventivo)
             .Include(mp => mp.Empleado)
             .Where(mp => mp.idEmpleado == idEmpleado)  // Filtra por el empleado asignado
             .ToListAsync();
        }

        public async Task<IEnumerable<MantenimientoPreventivo>> ConsultarMantenimientosPorPeriodo(DateTime inicio, DateTime fin)
        {
            return await _context.MantenimientoPreventivo
                .Include(mp => mp.Area)
                .Include(mp => mp.Maquina)
                .Include(mp => mp.FrecuenciaMP)
                .Include(mp => mp.EstatusPreventivo)
                .Where(mp => mp.fechaEjecucion >= inicio && mp.fechaEjecucion <= fin)
                .ToListAsync();
        }

        public async Task<MantenimientoPreventivo> ActivarMP(int idMP)
        {
            var mp = await _context.MantenimientoPreventivo.FindAsync(idMP);
            if (mp == null) return null;

            mp.activo = true;
            await _context.SaveChangesAsync();

            return mp;
        }

        public async Task<MantenimientoPreventivo> DesactivarMP(int idMP)
        {
            var mp = await _context.MantenimientoPreventivo.FindAsync(idMP);
            if (mp == null) return null;

            mp.activo = false; // Cambiar a false
            await _context.SaveChangesAsync();

            return mp;
        }

        public async Task<MantenimientoPreventivo> CambiarEstatusEnProceso(int idMP)
        {
            var mantenimiento = await _context.MantenimientoPreventivo
                .Include(mp => mp.Area)
                .Include(mp => mp.Maquina)
                .Include(mp => mp.Empleado)
                .FirstOrDefaultAsync(mp => mp.idMP == idMP);

            if (mantenimiento == null)
                return null;

            mantenimiento.idEstatusPreventivo = 5; // En proceso

            await _context.SaveChangesAsync();
            return mantenimiento;
        }

        public async Task<MantenimientoPreventivo> CancelarMantenimientoEnProceso(int idMP)
        {
            var mp = await _context.MantenimientoPreventivo
                .Include(mp => mp.Area)
                .Include(mp => mp.Maquina)
                .Include(mp => mp.Empleado)
                .FirstOrDefaultAsync(mp => mp.idMP == idMP);

            if (mp == null)
                return null;

            mp.idEstatusPreventivo = 1; // ← Pendiente
            await _context.SaveChangesAsync();

            return mp;
        }

        public async Task<IEnumerable<MantenimientoPreventivo>> ConsultarPorAnioYMes(int anio, int mes)
        {
            var fechaInicio = new DateTime(anio, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;

            var semanas = Enumerable.Range(0, (fechaFin - fechaInicio).Days + 1)
                .Select(offset =>
                {
                    var date = fechaInicio.AddDays(offset);
                    return new
                    {
                        Week = calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                        Year = date.Year
                    };
                })
                .Distinct()
                .ToList();

            var semanasDelMes = semanas.Select(x => x.Week).ToList();
            var aniosValidos = semanas.Select(x => x.Year).Distinct().ToList();

            return await _context.MantenimientoPreventivo
                .Where(mp =>
                    aniosValidos.Contains(mp.anioPreventivo) &&
                    semanasDelMes.Contains(mp.semanaPreventivo))
                .ToListAsync();
        }



    }
}
