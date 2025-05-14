using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class MantenimientoPreventivoEjecucionRepository(AppDbContext context): IMantenimientoPreventivoEjecucionRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<MantenimientoPreventivoEjecuciones> CrearAsync(MantenimientoPreventivoEjecuciones ejecucion)
        {
            _context.MantenimientoPreventivoEjecuciones.Add(ejecucion);
            await _context.SaveChangesAsync();
            return ejecucion;
        }

        public async Task<IEnumerable<MantenimientoPreventivoEjecuciones>> ObtenerPorMPAsync(int idMP)
        {
            return await _context.MantenimientoPreventivoEjecuciones
                .Where(e => e.idMP == idMP)
                .OrderByDescending(e => e.fechaEjecucion)
                .ToListAsync();
        }
    }
}
