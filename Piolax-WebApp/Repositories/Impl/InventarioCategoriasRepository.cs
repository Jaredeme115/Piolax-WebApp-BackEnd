using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class InventarioCategoriasRepository(AppDbContext context): IInventarioCategoriasRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<InventarioCategorias> RegistrarInventarioCategoria(InventarioCategorias inventarioCategoria)
        {
            _context.Add(inventarioCategoria);
            await _context.SaveChangesAsync();
            return inventarioCategoria;
        }

        public async Task<InventarioCategorias> Modificar(int idInventarioCategoria, InventarioCategorias inventarioCategoria)
        {
            _context.Update(inventarioCategoria);
            await _context.SaveChangesAsync();
            return inventarioCategoria;
        }

        public async Task<InventarioCategorias> Eliminar(int idInventarioCategoria)
        {
            var inventarioCategoria = await _context.InventarioCategorias.Where(p => p.idInventarioCategoria == idInventarioCategoria).FirstOrDefaultAsync();
            _context.Remove(inventarioCategoria);
            await _context.SaveChangesAsync();
            return inventarioCategoria;
        }

        public async Task<IEnumerable<InventarioCategorias>> ConsultarTodasCategorias()
        {
            var inventarioCategoria = await _context.InventarioCategorias.ToListAsync();
            return inventarioCategoria;
        }

        public async Task<InventarioCategorias?> ConsultarCategoriaPorNombre(string nombreInventarioCategoria)
        {
            return await _context.InventarioCategorias
                .FirstOrDefaultAsync(i => i.nombreInventarioCategoria == nombreInventarioCategoria);
        }

        public async Task<InventarioCategorias?> ConsultarCategoriaPorID(int idInventarioCategoria)
        {
            return await _context.InventarioCategorias
                .FirstOrDefaultAsync(i => i.idInventarioCategoria == idInventarioCategoria);
        }

        public async Task<bool> CategoriaExistePorNombre(string nombreInventarioCategoria)
        {
            return await _context.InventarioCategorias.AnyAsync(i => i.nombreInventarioCategoria == nombreInventarioCategoria);
        }

        public async Task<bool> CategoriaExistePorID(int idInventarioCategoria)
        {
            return await _context.InventarioCategorias.AnyAsync(i => i.idInventarioCategoria == idInventarioCategoria);
        }
    }
}
