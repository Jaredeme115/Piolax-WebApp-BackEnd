using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using SkiaSharp;

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
                var productoExistente = await _context.Inventario.FirstOrDefaultAsync(p => p.idRefaccion == inventario.idRefaccion);

                if (productoExistente == null)
                {
                    throw new InvalidOperationException("El producto no existe en el inventario.");
                }

                // Actualizar los valores del producto existente
                _context.Entry(productoExistente).CurrentValues.SetValues(inventario);

                await _context.SaveChangesAsync();
                return productoExistente;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Error al actualizar en la base de datos.", ex);
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

        //Metodo modificado para mostrar los detalles de la refaccion en base al nombre de la misma
        public async Task<InventarioDetalleDTO?> ConsultarRefaccionPorNombre(string nombreProducto)
        {
            var refaccion = await _context.Inventario
                .Include(i => i.InventarioCategorias)
                .Include(i => i.Areas)
                .Include(i => i.Maquinas)
                .FirstOrDefaultAsync(i => i.nombreProducto == nombreProducto);

            if (refaccion == null) return null;

            return new InventarioDetalleDTO
            {
                idRefaccion = refaccion.idRefaccion,
                descripcion = refaccion.descripcion,
                ubicacion = refaccion.ubicacion,
                idInventarioCategoria = refaccion.idInventarioCategoria,
                nombreInventarioCategoria = refaccion.InventarioCategorias?.nombreInventarioCategoria ?? "Sin categoría",
                cantidadActual = refaccion.cantidadActual,
                cantidadMax = refaccion.cantidadMax,
                cantidadMin = refaccion.cantidadMin,
                piezaCritica = refaccion.piezaCritica,
                nombreProducto = refaccion.nombreProducto,
                numParte = refaccion.numParte,
                proveedor = refaccion.proveedor,
                precioUnitario = refaccion.precioUnitario,
                precioInventarioTotal = refaccion.precioInventarioTotal,
                codigoQR = refaccion.codigoQR,
                proceso = refaccion.proceso,
                idArea = refaccion.idArea,
                nombreArea = refaccion.Areas?.nombreArea ?? "Sin área asignada",
                idMaquina = refaccion.idMaquina,
                nombreMaquina = refaccion.Maquinas?.nombreMaquina ?? "Sin máquina asignada",
                fechaEntrega = refaccion.fechaEntrega,
                inventarioActivoObsoleto = refaccion.inventarioActivoObsoleto,
                item = refaccion.item,
                fechaActualizacion = refaccion.fechaActualizacion,
                EstatusInventario = refaccion.EstatusInventario.ToString()
            };
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

        public async Task<IEnumerable<Inventario>> ConsultarRefaccionesPorFiltros(bool? piezaCritica, bool? inventarioActivoObsoleto, string? proceso)
        {
            // Partimos de un IQueryable para ir aplicando filtros condicionalmente
            var query = _context.Inventario.AsQueryable();

            if (piezaCritica.HasValue)
                query = query.Where(x => x.piezaCritica == piezaCritica.Value);

            if (inventarioActivoObsoleto.HasValue)
                query = query.Where(x => x.inventarioActivoObsoleto == inventarioActivoObsoleto.Value);

            if (!string.IsNullOrEmpty(proceso))
                query = query.Where(x => x.proceso == proceso);

            return await query.ToListAsync();
        }

        public async Task<InventarioDetalleDTO?> ConsultarRefaccionDetalle(int idInventario)
        {
            var refaccion = await _context.Inventario
                .Include(i => i.InventarioCategorias)
                .Include(i => i.Areas)
                .Include(i => i.Maquinas)
                .FirstOrDefaultAsync(i => i.idRefaccion == idInventario);

            if (refaccion == null) return null;

            return new InventarioDetalleDTO
            {
                descripcion = refaccion.descripcion,
                ubicacion = refaccion.ubicacion,
                idInventarioCategoria = refaccion.idInventarioCategoria,
                nombreInventarioCategoria = refaccion.InventarioCategorias?.nombreInventarioCategoria ?? "Sin categoría",
                cantidadActual = refaccion.cantidadActual,
                cantidadMax = refaccion.cantidadMax,
                cantidadMin = refaccion.cantidadMin,
                piezaCritica = refaccion.piezaCritica,
                nombreProducto = refaccion.nombreProducto,
                numParte = refaccion.numParte,
                proveedor = refaccion.proveedor,
                precioUnitario = refaccion.precioUnitario,
                precioInventarioTotal = refaccion.precioInventarioTotal,
                codigoQR = refaccion.codigoQR,
                proceso = refaccion.proceso,
                idArea = refaccion.idArea,
                nombreArea = refaccion.Areas?.nombreArea ?? "Sin área asignada",
                idMaquina = refaccion.idMaquina,
                nombreMaquina = refaccion.Maquinas?.nombreMaquina ?? "Sin máquina asignada",
                fechaEntrega = refaccion.fechaEntrega,
                inventarioActivoObsoleto = refaccion.inventarioActivoObsoleto,
                item = refaccion.item,
                fechaActualizacion = refaccion.fechaActualizacion,
                EstatusInventario = refaccion.EstatusInventario.ToString()
            };
        }


        public async Task<IEnumerable<string>> ConsultarNombresRefaccionesPorCategoria(int idCategoria)
        {
            return await _context.Inventario
                .Where(i => i.idInventarioCategoria == idCategoria)
                .Select(i => i.nombreProducto)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Inventario> inventario)
        {
            await _context.Set<Inventario>().AddRangeAsync(inventario); // Agrega el inventario al contexto.
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos.
        }

        public async Task<bool> ExisteProductoExacto(string nombreProducto, string numParte, string descripcion, int idArea, int idMaquina)
        {
            return await _context.Inventario.AnyAsync(p =>
                p.nombreProducto == nombreProducto &&
                p.numParte == numParte &&
                p.descripcion == descripcion &&
                p.idArea == idArea &&
                p.idMaquina == idMaquina
            );
    }
}
}
