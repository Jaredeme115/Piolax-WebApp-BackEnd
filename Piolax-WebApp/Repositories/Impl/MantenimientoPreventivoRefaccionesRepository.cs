using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class MantenimientoPreventivoRefaccionesRepository(AppDbContext context) : IMantenimientoPreventivoRefaccionesRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<MantenimientoPreventivo_Refacciones>> ConsultarRefaccionesMP(int idHistoricoMP)
        {
            return await _context.MantenimientoPreventivo_Refacciones
            .Where(x => x.idHistoricoMP == idHistoricoMP)
            .Include(x => x.Inventario) // Cargar datos de la refacción
            .ToListAsync();
        }

        public async Task<MantenimientoPreventivo_Refacciones> CrearRefaccionMP(MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones)
        {
            await _context.MantenimientoPreventivo_Refacciones.AddAsync(mantenimientoPreventivoRefacciones);
            await _context.SaveChangesAsync();
            return mantenimientoPreventivoRefacciones;
        }

        public async Task<IEnumerable<MantenimientoPreventivo_Refacciones>> ConsultarTodasLasRefacciones()
        {
            return await _context.MantenimientoPreventivo_Refacciones
                .Include(r => r.Inventario)
                .ToListAsync();
        }

        public async Task<bool> EliminarRefaccionMP(int idMPRefaccion)
        {
            var refaccion = await _context.MantenimientoPreventivo_Refacciones
                .FirstOrDefaultAsync(x => x.idMPRefaccion == idMPRefaccion);

            if (refaccion == null) return false;

            _context.MantenimientoPreventivo_Refacciones.Remove(refaccion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarRefaccionMP(MantenimientoPreventivo_Refacciones mantenimientoPreventivoRefacciones)
        {
            var existente = await _context.MantenimientoPreventivo_Refacciones
                .FirstOrDefaultAsync(x => x.idMPRefaccion == mantenimientoPreventivoRefacciones.idMPRefaccion);

            if (existente == null) return false;

            _context.Entry(existente).CurrentValues.SetValues(mantenimientoPreventivoRefacciones);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MantenimientoPreventivo_Refacciones?> ConsultarRefaccionPorId(int idMPRefaccion)
        {
            return await _context.MantenimientoPreventivo_Refacciones
                .Include(r => r.Inventario)
                .FirstOrDefaultAsync(x => x.idMPRefaccion == idMPRefaccion);
        }
    }
}
