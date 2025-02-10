using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class AsignacionRepository(AppDbContext context) : IAsignacionRepository
    {
        private readonly AppDbContext _context = context;

        // Métodos del repositorio
        public async Task<Asignaciones> AgregarAsignacion(Asignaciones asignaciones)
        {
            await _context.Asignaciones.AddAsync(asignaciones);
            await _context.SaveChangesAsync();
            return asignaciones;
        }

        public async Task<IEnumerable<Asignaciones>> ConsultarTodasLasAsignaciones()
        {
            return await _context.Asignaciones.ToListAsync();
        }

        public async Task<Asignaciones?> ConsultarAsignacionPorId(int idAsignacion)
        {
            return await _context.Asignaciones
            .Include(a => a.Solicitud) // Cargar la propiedad Solicitud
            .FirstOrDefaultAsync(a => a.idAsignacion == idAsignacion);
        }

        public async Task<Asignaciones> ActualizarAsignacion(int idAsignacion, Asignaciones asignaciones)
        {
           
            var asignacionExistente = await _context.Asignaciones.FindAsync(idAsignacion);

           
            if (asignacionExistente == null)
            {
                return null; 
            }

           
            _context.Entry(asignacionExistente).CurrentValues.SetValues(asignaciones);

           
            await _context.SaveChangesAsync();

            return asignacionExistente;
        }

        public async Task<bool> EliminarAsignacion(int idAsignacion)
        {
            var asignacion = await _context.Asignaciones.FindAsync(idAsignacion);
            if (asignacion == null)
            {
                return false;
            }
            _context.Asignaciones.Remove(asignacion);
            await _context.SaveChangesAsync();
            return true;
        }

        // Utilidades
        public async Task<bool> AsignacionExiste(int idAsignacion)
        {
            return await _context.Asignaciones.AnyAsync(a => a.idAsignacion == idAsignacion);
        }

        public async Task<IEnumerable<Asignaciones>> ConsultarAsignacionesCompletadas(int idMaquina, int idArea, int? idEmpleado)
        {
            // Suponemos que un idStatusAsignacion == 4 indica que la asignación está finalizada.
            var query = _context.Asignaciones
                .Include(a => a.Solicitud)
                .Include(a => a.Asignacion_Tecnico)
                .Where(a => a.Solicitud.idMaquina == idMaquina &&
                            a.Solicitud.idAreaSeleccionada == idArea &&
                            a.idStatusAsignacion == 3);

            if (idEmpleado.HasValue)
            {
                query = query.Where(a => a.Asignacion_Tecnico.Any(t => t.idEmpleado == idEmpleado.Value));
            }

            return await query.ToListAsync();
        }
    }
}
