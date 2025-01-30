using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class AsignacionRepository: IAsignacionRepository
    {
        private readonly AppDbContext _context;

        public AsignacionRepository(AppDbContext context)
        {
            _context = context;
        }

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

        public async Task<Asignaciones> ConsultarAsignacionPorId(int idAsignacion)
        {
            return await _context.Asignaciones.FindAsync(idAsignacion);
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
    }
}
