using Piolax_WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Piolax_WebApp.Repositories.Impl
{
    public class StatusEmpleadoRepository(AppDbContext context) : IStatusEmpleadoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<StatusEmpleado> Consultar(int idStatusEmpleado)
        {
            return await _context.StatusEmpleado.Where(p => p.idStatusEmpleado == idStatusEmpleado).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StatusEmpleado>> ConsultarTodos()
        {
            var statusEmpleado = await _context.StatusEmpleado.ToListAsync();
            return statusEmpleado;
        }

        public async Task<StatusEmpleado> Registro(StatusEmpleado statusEmpleado)
        {
            await _context.StatusEmpleado.AddAsync(statusEmpleado);
            await _context.SaveChangesAsync();
            return statusEmpleado;
        }

        public async Task<StatusEmpleado> Modificar(int idStatusEmpleado, StatusEmpleado statusEmpleado)
        {
            _context.Update(statusEmpleado);
            await _context.SaveChangesAsync();
            return statusEmpleado;
        }

        public async Task<StatusEmpleado> Eliminar(int idStatusEmpleado)
        {
            var statusEmpleado = await _context.StatusEmpleado.Where(p => p.idStatusEmpleado == idStatusEmpleado).FirstOrDefaultAsync();
            if (statusEmpleado != null)
            {
                _context.Remove(statusEmpleado);
                await _context.SaveChangesAsync();
            }
            return statusEmpleado;
        }

        public Task<bool> StatusEmpleadoExiste(int idStatusEmpleado)
        {
            return _context.StatusEmpleado.AnyAsync(p => p.idStatusEmpleado == idStatusEmpleado);
        }

        public Task<bool> StatusEmpleadoExisteRegistro(string descripcionStatusEmpleado)
        {
            return _context.StatusEmpleado.AnyAsync(p => p.descripcionStatusEmpleado == descripcionStatusEmpleado);
        }

    }
}
