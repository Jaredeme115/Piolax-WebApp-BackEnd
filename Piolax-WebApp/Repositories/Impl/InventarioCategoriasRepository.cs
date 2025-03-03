using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class InventarioCategoriasRepository(AppDbContext context): IInventarioCategoriasRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<InventarioCategorias> RegistrarInventarioCategoria(InventarioCategorias inventarioCategoria)
        {
            try
            {
                _context.Add(inventarioCategoria);
                int cambios = await _context.SaveChangesAsync();
                return cambios > 0 ? inventarioCategoria : null;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al guardar la categoría en la base de datos.", ex);
            }
        }

        public async Task<InventarioCategorias?> Modificar(int idInventarioCategoria, InventarioCategorias inventarioCategoria)
        {
            if (inventarioCategoria == null)
            {
                throw new ArgumentNullException(nameof(inventarioCategoria), "La categoría no puede ser nula.");
            }

            try
            {
                _context.InventarioCategorias.Update(inventarioCategoria);
                var filasAfectadas = await _context.SaveChangesAsync();

                return filasAfectadas > 0 ? inventarioCategoria : null;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar la base de datos.", ex);
            }
        }

        public async Task<InventarioCategorias> Eliminar(int idInventarioCategoria)
        {
            var inventarioCategoria = await _context.InventarioCategorias
         .Where(p => p.idInventarioCategoria == idInventarioCategoria)
         .FirstOrDefaultAsync();

            if (inventarioCategoria == null)
            {
                return null; // Para que el controlador devuelva 404 Not Found
            }

            try
            {
                _context.InventarioCategorias.Remove(inventarioCategoria);
                await _context.SaveChangesAsync();
                return inventarioCategoria;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al eliminar la categoría en la base de datos.", ex);
            }
        }

        /*public async Task<IEnumerable<InventarioCategorias>> ConsultarTodasCategorias()
        {
            return await _context.InventarioCategorias
             .Include(ic => ic.Inventario) // Incluir las piezas asociadas a la categoría
             .ToListAsync();
        }*/

        public async Task<IEnumerable<string>> ConsultarNombresCategorias()
        {
            return await _context.InventarioCategorias
                .Select(c => c.nombreInventarioCategoria)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> ConsultarTodasCategorias()
        {
            return await _context.InventarioCategorias
             .Select(cn => cn.nombreInventarioCategoria ) 
             .ToListAsync();
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
