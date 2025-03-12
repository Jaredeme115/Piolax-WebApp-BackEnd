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

        public async Task AgregarAreaYRol(string numNomina, int idArea, int idRol, bool esAreaPrincipal)
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
                    idRol = idRol,
                    esAreaPrincipal = esAreaPrincipal
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

        public async Task<IEnumerable<Roles>> ObtenerRolPorEmpleadoYArea(string numNomina, int idArea)
        {
            return await _context.EmpleadoAreaRol
            .Where(e => e.Empleado.numNomina == numNomina && e.idArea == idArea)
            .Select(e => e.Rol)
            .ToListAsync();

        }

        public async Task<IEnumerable<Roles>> ObtenerRolesPorEmpleado(string numNomina)
        {
            return await _context.EmpleadoAreaRol
                .Where(e => e.Empleado.numNomina == numNomina) // Filtra por número de nómina
                .Select(e => e.Rol) // Selecciona solo los roles
                .Distinct() // Evita duplicados si un empleado tiene el mismo rol en distintas áreas
                .ToListAsync();
        }

        public async Task<bool> ValidarRolPorEmpleadoYArea(string numNomina, int idArea)
        {
            var empleadoAreaRol = await _context.EmpleadoAreaRol
                .Include(e => e.Rol)
                .FirstOrDefaultAsync(e => e.Empleado.numNomina == numNomina && e.idArea == idArea);

            return empleadoAreaRol != null;
        }

        public async Task<bool> TieneAreaPrincipal(string numNomina)
        {
            return await _context.EmpleadoAreaRol
                .AnyAsync(e => e.Empleado.numNomina == numNomina && e.esAreaPrincipal);
        }

        //Metodo para obtener el area de un empleado
        public async Task<IEnumerable<Areas>> ObtenerAreaPorEmpleado(string numNomina)
        {
            return await _context.EmpleadoAreaRol
                .Include(e => e.Area)
                .Where(e => e.Empleado.numNomina == numNomina)
                .Select(e => e.Area)
                .ToListAsync();
        }

        //Metodo para obtener la informacion detallada de todos los empleados (area y rol incluido)
        public async Task<IEnumerable<EmpleadoAreaRol>> ConsultarTodosConDetalles()
        {
            return await _context.EmpleadoAreaRol
                .Include(e => e.Empleado)
                .Include(e => e.Area)
                .Include(e => e.Rol)
                .ToListAsync();
        }

        public async Task<EmpleadoInfoDTO> ConsultarEmpleadoConDetalles(string numNomina)
        {
            var empleado = await _context.Empleado
                .Include(e => e.EmpleadoAreaRol)
                    .ThenInclude(ear => ear.Area)
                .Include(e => e.EmpleadoAreaRol)
                    .ThenInclude(ear => ear.Rol)
                .Where(e => e.numNomina == numNomina)
                .Select(e => new EmpleadoInfoDTO
                {
                    numNomina = e.numNomina,
                    nombre = e.nombre,
                    apellidoPaterno = e.apellidoPaterno,
                    apellidoMaterno = e.apellidoMaterno,
                    telefono = e.telefono,
                    email = e.email,
                    fechaIngreso = e.fechaIngreso,
                    idStatusEmpleado = e.idStatusEmpleado,
                    areaPrincipal = e.EmpleadoAreaRol
                        .Where(ear => ear.esAreaPrincipal)
                        .Select(ear => new AreaRolDTO
                        {
                            idArea = ear.idArea,
                            Area = ear.Area.nombreArea,
                            idRol = ear.idRol,
                            Rol = ear.Rol.nombreRol
                        }).FirstOrDefault()
                }).FirstOrDefaultAsync();

            if (empleado != null)
            {
                empleado.SetIdAreaRol(); // ✅ Llama a la función para asignar idArea e idRol
            }

            return empleado;
        }

        public async Task<IEnumerable<EmpleadoNombreCompletoDTO>> ObtenerEmpleadosPorArea(int idArea)
        {
            return await _context.EmpleadoAreaRol
            .Include(ear => ear.Empleado) // Incluye datos del empleado
            .Where(ear => ear.idArea == idArea) // Filtra por id de área
            .Select(ear => new EmpleadoNombreCompletoDTO
            {
                idEmpleado = ear.Empleado.idEmpleado,
                nombre = ear.Empleado.nombre,
                apellidoPaterno = ear.Empleado.apellidoPaterno,
                apellidoMaterno = ear.Empleado.apellidoMaterno
            })
            .ToListAsync();
        }



    }
}
