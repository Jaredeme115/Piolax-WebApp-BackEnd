using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class StatusOrdenRepository(AppDbContext context) : IStatusOrdenRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<StatusOrden> Consultar(int idStatusOrden)
        {
            return await _context.StatusOrden.Where(p => p.idStatusOrden == idStatusOrden).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StatusOrden>> ConsultarTodos()
        {
            var statusOrden = await _context.StatusOrden.ToListAsync();
            return statusOrden;
        }

        public async Task<StatusOrden> Registro(StatusOrden statusOrden)
        {
            _context.StatusOrden.Add(statusOrden);
            await _context.SaveChangesAsync();
            return statusOrden;
        }

        public async Task<StatusOrden> Modificar(int idStatusOrden, StatusOrden statusOrden)
        {
            _context.Entry(statusOrden).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return statusOrden;
        }

        public async Task<StatusOrden> Eliminar(int idStatusOrden)
        {
            var statusOrden = await _context.StatusOrden.Where(p => p.idStatusOrden == idStatusOrden).FirstOrDefaultAsync();
            _context.StatusOrden.Remove(statusOrden);
            await _context.SaveChangesAsync();
            return statusOrden;
        }

        public async Task<bool> StatusOrdenExiste(int idStatusOrden)
        {
            return await _context.StatusOrden.AnyAsync(p => p.idStatusOrden == idStatusOrden);
        }

        public async Task<bool> StatusOrdenExisteRegistro(string descripcionStatusOrden)
        {
            return await _context.StatusOrden.AnyAsync(p => p.descripcionStatusOrden == descripcionStatusOrden);
        }



    }
}
