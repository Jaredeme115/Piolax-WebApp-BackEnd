using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
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

        public async Task ModificarEmpleadoAreaRol(Empleado empleado, EmpleadoAreaRol empleadoAreaRol)
        {
            // Actualizar el empleado
            _context.Empleado.Update(empleado);

            // Buscar el registro existente de EmpleadoAreaRol
            var empleadoAreaRolExistente = await _context.EmpleadoAreaRol
                .FirstOrDefaultAsync(e => e.idEmpleado == empleado.idEmpleado);

            if (empleadoAreaRolExistente != null)
            {
                // Eliminar el registro existente
                _context.EmpleadoAreaRol.Remove(empleadoAreaRolExistente);
                await _context.SaveChangesAsync();
            }

            // Agregar el nuevo registro
            await _context.EmpleadoAreaRol.AddAsync(empleadoAreaRol);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
        }

        public async Task AgregarAreaYRol(string numNomina, int idArea, int idRol)
        {
            // Buscar el empleado por número de nómina
            var empleado = await _context.Empleado
                .FirstOrDefaultAsync(e => e.numNomina == numNomina);

            if (empleado != null)
            {
                // Crear un nuevo registro de EmpleadoAreaRol
                var empleadoAreaRol = new EmpleadoAreaRol
                {
                    idEmpleado = empleado.idEmpleado,
                    idArea = idArea,
                    idRol = idRol
                };

                // Agregar el nuevo registro
                await _context.EmpleadoAreaRol.AddAsync(empleadoAreaRol);

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarAreaYRol(string numNomina, int idArea, int idRol)
        {
            // Buscar el empleado por número de nómina
            var empleado = await _context.Empleado
                .FirstOrDefaultAsync(e => e.numNomina == numNomina);

            if (empleado != null)
            {
                // Buscar el registro de EmpleadoAreaRol
                var empleadoAreaRol = await _context.EmpleadoAreaRol
                    .FirstOrDefaultAsync(ear => ear.idEmpleado == empleado.idEmpleado
                        && ear.idArea == idArea
                        && ear.idRol == idRol);

                if (empleadoAreaRol != null)
                {
                    // Eliminar el registro
                    _context.EmpleadoAreaRol.Remove(empleadoAreaRol);

                    // Guardar los cambios en la base de datos
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<IEnumerable<EmpleadoAreaRol>> ObtenerAreasRolesPorEmpleado(string numNomina)
        {
            return await _context.EmpleadoAreaRol
                .Include(ear => ear.Area) // Incluye datos del área
                .Include(ear => ear.Rol)  // Incluye datos del rol
                .Include(ear => ear.Empleado) // Incluye datos del empleado
                .Where(ear => ear.Empleado.numNomina == numNomina) // Filtra por número de nómina
                .ToListAsync();
        }

        public async Task<string?> ObtenerRolPorEmpleadoYArea(string numNomina, int idArea)
        {
            var empleadoAreaRol = await _context.EmpleadoAreaRol
                .Include(e => e.Rol)
                .FirstOrDefaultAsync(e => e.Empleado.numNomina == numNomina && e.idArea == idArea);

            return empleadoAreaRol?.Rol?.nombreRol;

        }
    }
}
