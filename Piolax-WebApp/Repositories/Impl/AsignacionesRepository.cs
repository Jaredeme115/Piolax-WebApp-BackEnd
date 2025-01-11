using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class AsignacionesRepository(AppDbContext context): IAsignacionesRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Asignaciones> RegistrarAsignacion(Asignaciones asignaciones)
        {
            _context.Asignaciones.Add(asignaciones);
            await _context.SaveChangesAsync();
            return asignaciones;
        }

        public async Task<Asignaciones> ObtenerAsignacionConDetalles(int idAsignacion)
        {
            return await _context.Asignaciones
                .Include(a => a.idSolicitud)
                .Include(a => a.idEmpleado)
                .Include(a => a.horaInicio)
                .Include(a => a.horaTermino)
                .Include(a => a.solucion)
                .Include(a => a.idRefaccion)
                .Include(a => a.cantidad)
                .Include(a => a.maquinaDetenida)
                .Include(a => a.CategoriaAsignacion)
                .Include(a => a.StatusAprobacionTecnico)
                .FirstOrDefaultAsync(a => a.idAsignacion == idAsignacion);
        }

        public async Task<IEnumerable<Asignaciones>> ObtenerTodasLasAsignaciones()
        {
            return await _context.Asignaciones
                .Include(a => a.Solicitud)
                .Include(a => a.Empleado)
                .Include(a => a.Inventario)
                .Include(a => a.CategoriaAsignacion)
                .Include(a => a.StatusAprobacionTecnico)
                .Include(a => a.Asignacion_Refacciones)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asignaciones>> ObtenerAsignacionesPorTecnico(string numNomina)
        {
            return await _context.Asignaciones
                .Include(a => a.Solicitud)
                .Include(a => a.Empleado)
                .Include(a => a.Inventario)
                .Include(a => a.CategoriaAsignacion)
                .Include(a => a.StatusAprobacionTecnico)
                .Include(a => a.Asignacion_Refacciones)
                .Where(a => a.Empleado.numNomina == numNomina)
                .ToListAsync();
        }

    
    }
}
