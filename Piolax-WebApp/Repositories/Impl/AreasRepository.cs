using Piolax_WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Piolax_WebApp.Repositories.Impl
{
    public class AreasRepository(AppDbContext context): IAreasRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Areas> Consultar(int idArea)
        {
            return await _context.Areas.Where(p => p.idArea == idArea).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Areas>> ConsultarTodos()
        {
            var areas = await _context.Areas.ToListAsync();
            return areas;
        }

        public async Task<Areas> Registro(Areas area)
        {
            await _context.Areas.AddAsync(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<Areas> Modificar(int idArea, Areas area)
        {
            _context.Update(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<Areas> Eliminar(int idArea)
        {
            var area = await _context.Areas.Where(p => p.idArea == idArea).FirstOrDefaultAsync();
            if (area != null)
            {
                _context.Remove(area);
                await _context.SaveChangesAsync();
            }
            return area;
        }

        public Task<bool> AreaExiste(int idArea)
        {
            return _context.Areas.AnyAsync(p => p.idArea == idArea);
        }

        public Task<bool> AreaExisteRegistro(string nombreArea)
        {
            return _context.Areas.AnyAsync(p => p.nombreArea == nombreArea);
        }

        public async Task ActualizarContadorMaquinasActivas(int idArea)
        {
            var conteo = await _context.Maquinas
                .Where(m => m.idArea == idArea && m.maquinaActiva)
                .CountAsync();

            var area = await _context.Areas.FindAsync(idArea);
            if (area != null)
            {
                area.contadorMaquinasActivasPorArea = conteo;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RecalcularTodosLosContadores()
        {
            var areas = await _context.Areas.Include(a => a.Maquinas).ToListAsync();

            foreach (var area in areas)
            {
                area.contadorMaquinasActivasPorArea = area.Maquinas.Count(m => m.maquinaActiva);
            }

            await _context.SaveChangesAsync();
        }


    }
}
