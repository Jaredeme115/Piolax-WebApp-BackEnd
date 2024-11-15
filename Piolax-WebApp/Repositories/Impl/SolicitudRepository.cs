using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class SolicitudRepository(AppDbContext context) : ISolicitudesRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Solicitudes> Consultar(int idSolicitud)
        {
            return await _context.Solicitudes.Where(p => p.idSolicitud == idSolicitud).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Solicitudes>> ConsultarTodos()
        {
            var solicitudes = await _context.Solicitudes.ToListAsync();
            return solicitudes;
        }

        public async Task<Solicitudes> Registro(Solicitudes solicitudes)
        {
            await _context.Solicitudes.AddAsync(solicitudes);
            await _context.SaveChangesAsync();
            return solicitudes;
        }

        public async Task<Solicitudes> Modificar(int idSolicitud, Solicitudes solicitudes)
        {
            _context.Update(solicitudes);
            await _context.SaveChangesAsync();
            return solicitudes;
        }

        public async Task<Solicitudes> Eliminar(int idSolicitud)
        {
            var solicitud = await _context.Solicitudes.Where(p => p.idSolicitud == idSolicitud).FirstOrDefaultAsync();
            if (solicitud != null)
            {
                _context.Remove(solicitud);
                await _context.SaveChangesAsync();
            }
            return solicitud;
        }

        public Task<bool> SolicitudExiste(int idSolicitud)
        {
            return _context.Solicitudes.AnyAsync(p => p.idSolicitud == idSolicitud);
        }

    }
}
