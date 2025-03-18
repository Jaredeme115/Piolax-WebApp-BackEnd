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
            mantenimientoExistente.idFrecuenciaPreventivo = mantenimientoPreventivo.idFrecuenciaPreventivo;
            mantenimientoExistente.idEstatusPreventivo = mantenimientoPreventivo.idEstatusPreventivo;
            mantenimientoExistente.Activo = mantenimientoPreventivo.Activo;
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
                .ToListAsync();
        }

    }
}
