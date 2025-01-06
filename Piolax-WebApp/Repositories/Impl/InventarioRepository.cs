using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class InventarioRepository(AppDbContext context): IInventarioRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Inventario> RegistrarInventario(Inventario inventario)
        {
            _context.Add(inventario);
            await _context.SaveChangesAsync();
            return inventario;
        }

        public async Task<Inventario> Modificar(int idRefaccion, Inventario inventario)
        {
            _context.Update(inventario);
            await _context.SaveChangesAsync();
            return inventario;
        }

        public async Task<Inventario> Eliminar(int idRefaccion)
        {
            var inventario = await _context.Inventario.Where(p => p.idRefaccion == idRefaccion).FirstOrDefaultAsync();
            _context.Remove(inventario);
            await _context.SaveChangesAsync();
            return inventario;
        }

        public async Task<IEnumerable<Inventario>> ConsultarTodoInventario()
        {
            var inventario = await _context.Inventario.ToListAsync();
            return inventario;
        }

        public async Task<Inventario?> ConsultarInventarioConDetalles(int idRefaccion)
        {

            return await _context.Inventario
                .Include(i => i.descripcion)
                .Include(i => i.ubicacion)
                .Include(i => i.categoria)
                .Include(i => i.cantidadActual)
                .Include(i => i.cantidadMin)
                .Include(i => i.cantidadMax)
                .Include(i => i.piezaCritica)
                .Include(i => i.nombreProducto)
                .Include(i => i.numParte)
                .Include(i => i.proveedor)
                .Include(i => i.precioUnitario)
                .Include(i => i.precioInventarioTotal)
                .Include(i => i.codigoBarra)
                .Include(i => i.codigoQR)
                .Include(i => i.proceso)
                .Include(i => i.idArea)
                .Include(i => i.idMaquina)
                .Include(i => i.fechaEntrega)
                .Include(i => i.inventarioActivoObsoleto)
                .FirstOrDefaultAsync(i => i.idRefaccion == idRefaccion);
        }

        public async Task<Inventario?> ConsultarInventarioPorNombre(string nombreProducto)
        {
            return await _context.Inventario.Where(p => p.nombreProducto == nombreProducto).FirstOrDefaultAsync();
        }

        public async Task<Inventario?> ConsultarInventarioPorCategoria(string categoria)
        {
            return await _context.Inventario.Where(p => p.categoria == categoria).FirstOrDefaultAsync();
        }


    }
}
