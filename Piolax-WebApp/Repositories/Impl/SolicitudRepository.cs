using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class SolicitudRepository(AppDbContext context) : ISolicitudesRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Solicitudes> RegistrarSolicitud(Solicitudes solicitudes)
        {
            _context.Solicitudes.Add(solicitudes);
            await _context.SaveChangesAsync();
            return solicitudes;
        }

        public async Task<Solicitudes> ObtenerSolicitudConDetalles(int idSolicitud)
        {
            return await _context.Solicitudes
                .Include(s => s.Empleado)
                .ThenInclude(e => e.EmpleadoAreaRol)
                .ThenInclude(ar => ar.Area)
                .Include(s => s.Empleado)
                .ThenInclude(e => e.EmpleadoAreaRol)
                .ThenInclude(ar => ar.Rol)
                .FirstOrDefaultAsync(s => s.idSolicitud == idSolicitud);
               
        }

        public async Task<IEnumerable<Solicitudes>> ObtenerSolicitudes()
        {
            return await _context.Solicitudes
                .Include(s => s.Empleado)
                .ThenInclude(e => e.EmpleadoAreaRol)
                .ThenInclude(ar => ar.Area)
                .Include(s => s.Empleado)
                .ThenInclude(e => e.EmpleadoAreaRol)
                .ThenInclude(ar => ar.Rol)
                .ToListAsync();
        }


    }
}
