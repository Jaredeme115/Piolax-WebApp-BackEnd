using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class InventarioRepository(AppDbContext context): IInventarioRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Inventario> RegistrarInventario(Inventario inventario)
        {
            if (inventario == null)
            {
                throw new ArgumentNullException(nameof(inventario), "El objeto Inventario no puede ser nulo.");
            }

            try
            {
                var existeProducto = await _context.Inventario.AnyAsync(p => p.numParte == inventario.numParte);
                if (existeProducto)
                {
                    throw new InvalidOperationException("El producto ya está registrado en el inventario.");
                }

                _context.Add(inventario);
                await _context.SaveChangesAsync();
                return inventario;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al registrar en la base de datos.", ex);
            }
        }

        public async Task<Inventario> Modificar(Inventario inventario)
        {
            if (inventario == null)
            {
                throw new ArgumentNullException(nameof(inventario), "El objeto Inventario no puede ser nulo.");
            }

            try
            {
                var existeProducto = await _context.Inventario.AnyAsync(p => p.numParte == inventario.numParte);
                if (existeProducto)
                {
                    throw new InvalidOperationException("El producto ya está registrado en el inventario.");
                }

                _context.Add(inventario);
                await _context.SaveChangesAsync();
                return inventario;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al registrar en la base de datos.", ex);
            }
        }

        public async Task<Inventario?> Eliminar(int idRefaccion)
        {
            var inventario = await _context.Inventario
                .Where(p => p.idRefaccion == idRefaccion)
                .FirstOrDefaultAsync();

            if (inventario == null)
            {
                return null; // Indicar que no existe
            }

            _context.Inventario.Remove(inventario);

            try
            {
                await _context.SaveChangesAsync();
                return inventario; // Devuelve el producto eliminado si todo fue bien
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al eliminar la refacción.", ex);
            }
        }

        public async Task<IEnumerable<Inventario>> ConsultarTodoInventario()
        {
            var inventario = await _context.Inventario.ToListAsync();
            return inventario;
        }

        /*public async Task<Inventario?> ConsultarInventarioConDetalles(int idRefaccion)
        {

            return await _context.Inventario
                .Include(i => i.descripcion)
                .Include(i => i.ubicacion)
                .Include(i => i.idInventarioCategoria)
                .Include(i => i.cantidadActual)
                .Include(i => i.cantidadMin)
                .Include(i => i.cantidadMax)
                .Include(i => i.piezaCritica)
                .Include(i => i.nombreProducto)
                .Include(i => i.numParte)
                .Include(i => i.proveedor)
                .Include(i => i.precioUnitario)
                .Include(i => i.precioInventarioTotal)
                .Include(i => i.codigoBarras)
                .Include(i => i.codigoQR)
                .Include(i => i.proceso)
                .Include(i => i.idArea)
                .Include(i => i.idMaquina)
                .Include(i => i.fechaEntrega)
                .Include(i => i.inventarioActivoObsoleto)
                .FirstOrDefaultAsync(i => i.idRefaccion == idRefaccion);
        }*/

        public async Task<Inventario?> ConsultarInventarioPorNombre(string nombreProducto)
        {
            return await _context.Inventario.Where(p => p.nombreProducto == nombreProducto).FirstOrDefaultAsync();
        }

        public async Task<Inventario?> ConsultarInventarioPorCategoria(int idInventarioCategoria)
        {
            return await _context.Inventario.Where(p => p.idInventarioCategoria == idInventarioCategoria).FirstOrDefaultAsync();
        }

        public async Task<Inventario?> ConsultarInventarioPorID(int idRefaccion)
        {
            return await _context.Inventario.Where(p => p.idRefaccion == idRefaccion).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Inventario>> ConsultarTodosLosProductosPorCategoria(int idInventarioCategoria)
        {
            return await _context.Inventario.Where(p => p.idInventarioCategoria == idInventarioCategoria).ToListAsync();
        }

        public async Task<bool> ExisteProductoInventario(int idRefaccion)
        {
            return await _context.Inventario.AnyAsync(p => p.idRefaccion == idRefaccion);
        }

        public async Task<bool> ExisteNumParte(string numParte)
        {
            return await _context.Inventario.AnyAsync(p => p.numParte == numParte);
        }

        public async Task ActualizarCantidadInventario(int idRefaccion, int cantidadADescontar)
        {
            var inventario = await _context.Inventario.FirstOrDefaultAsync(i => i.idRefaccion == idRefaccion);

            if (inventario == null)
            {
                throw new Exception($"No se encontró el inventario con idRefaccion: {idRefaccion}");
            }

            if (inventario.cantidadActual < cantidadADescontar)
            {
                throw new Exception($"No hay suficiente inventario disponible para la refacción con idRefaccion: {idRefaccion}");
            }

            inventario.cantidadActual -= cantidadADescontar;
            await _context.SaveChangesAsync();
        }

        public async Task<int> ConsultarCantidadDisponible(int idRefaccion)
        {
            var inventario = await _context.Inventario.FirstOrDefaultAsync(i => i.idRefaccion == idRefaccion);

            if (inventario == null)
            {
                throw new Exception($"No se encontró el inventario con idRefaccion: {idRefaccion}");
            }

            return inventario.cantidadActual;
        }
    }
}
