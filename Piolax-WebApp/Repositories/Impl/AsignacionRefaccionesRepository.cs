using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class AsignacionRefaccionesRepository(AppDbContext context) : IAsignacionRefaccionesRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<asignacion_refacciones>> ConsultarRefaccionesPorAsignacion(int idAsignacion)
        {
            return await _context.asignacion_refacciones
            .Where(x => x.idAsignacion == idAsignacion)
            .Include(x => x.Inventario) // Cargar datos de la refacción
            .ToListAsync();
        }

        public async Task<IEnumerable<asignacion_refacciones>> ConsultarRefaccionesPorSolicitud(int idSolicitud)
        {
            return await _context.asignacion_refacciones
                .Where(r => _context.Asignaciones
                    .Where(a => a.idSolicitud == idSolicitud)
                    .Select(a => a.idAsignacion)
                    .Contains(r.idAsignacion))
                .Include(r => r.Inventario)
                .ToListAsync();
        }

       



        public async Task<asignacion_refacciones> CrearAsignacionRefacciones(asignacion_refacciones asignacionRefacciones)
        {
            await _context.asignacion_refacciones.AddAsync(asignacionRefacciones);
            await _context.SaveChangesAsync();
            return asignacionRefacciones;
        }

        public async Task<IEnumerable<asignacion_refacciones>> ConsultarTodasLasRefacciones()
        {
            return await _context.asignacion_refacciones
                .Include(r => r.Inventario)
                .Include(r => r.Asignacion_Tecnicos)
                .ToListAsync();
        }

        public async Task<bool> EliminarRefaccionDeAsignacion(int idAsignacionRefaccion)
        {
            // Buscar la asignación de la refacción por su ID
            var asignacionRefaccion = await _context.asignacion_refacciones.FindAsync(idAsignacionRefaccion);
            // Si no se encuentra la asignación de la refacción, devolver false
            if (asignacionRefaccion == null)
            {
                return false;
            }
            // Eliminar la asignación de la refacción del contexto
            _context.asignacion_refacciones.Remove(asignacionRefaccion);
            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
            // Devolver true para indicar que la eliminación fue exitosa
            return true;
        }

        public async Task<bool> ActualizarRefaccionEnAsignacion(asignacion_refacciones asignacionRefacciones)
        {
            var existente = await _context.asignacion_refacciones.FindAsync(asignacionRefacciones.idAsignacionRefaccion);
            if (existente == null) return false;

            _context.Entry(existente).CurrentValues.SetValues(asignacionRefacciones);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<asignacion_refacciones?> ConsultarRefaccionesPorId(int idAsignacionRefaccion)
        {
            return await _context.asignacion_refacciones.FindAsync(idAsignacionRefaccion);
        }
    }
}
