using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class EmpleadoAreaRolRepository(AppDbContext context) : IEmpleadoAreaRolRepository
    {
        private readonly AppDbContext _context = context;

        public async Task RegistrarEmpleadoYAsignarAreaRol(Empleado empleado, EmpleadoAreaRol empleadoAreaRol)
        {
            await _context.Empleado.AddAsync(empleado);
            await _context.SaveChangesAsync();

            empleadoAreaRol.idEmpleado = empleado.idEmpleado; // Asegura que tenga el ID del empleado creado
            await _context.EmpleadoAreaRol.AddAsync(empleadoAreaRol);
            await _context.SaveChangesAsync();
        }
    }
}
