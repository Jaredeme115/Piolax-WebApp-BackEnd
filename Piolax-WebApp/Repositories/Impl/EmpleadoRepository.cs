using Piolax_WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Piolax_WebApp.Repositories.Impl
{
    public class EmpleadoRepository(AppDbContext context) : IEmpleadoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Empleado> Agregar(Empleado empleado)
        {
            _context.Add(empleado);
            await _context.SaveChangesAsync();
            return empleado;
        }

        public async Task<Empleado> Consultar(string numNomina)
        {
            return await _context.Empleado.Where(p => p.numNomina == numNomina).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Empleado>> ConsultarTodos()
        {
            var empleados = await _context.Empleado.ToListAsync();
            return empleados;
        }

        public Task<bool> EmpleadoExiste(string numNomina)
        {
            return _context.Empleado.AnyAsync(empleado => empleado.numNomina == numNomina.ToLower());
        }

        public Empleado EmpleadoExisteLogin(string numNomina)
        {
            return _context.Empleado.SingleOrDefault(empleado => empleado.numNomina.ToLower() == numNomina.ToLower());
        }

    }
}
