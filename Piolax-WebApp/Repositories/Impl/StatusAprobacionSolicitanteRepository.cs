using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class StatusAprobacionSolicitanteRepository(AppDbContext context) : IStatusAprobacionSolicitanteRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<StatusAprobacionSolicitante> Consultar(int idStatusAprobacionSolicitante)
        {
            return await _context.StatusAprobacionSolicitante.Where(p => p.idStatusAprobacionSolicitante == idStatusAprobacionSolicitante).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StatusAprobacionSolicitante>> ConsultarTodos()
        {
            var statusAprobacionSolicitante = await _context.StatusAprobacionSolicitante.ToListAsync();
            return statusAprobacionSolicitante;
        }

        public async Task<StatusAprobacionSolicitante> Registro(StatusAprobacionSolicitante statusAprobacionSolicitante)
        {
            _context.StatusAprobacionSolicitante.Add(statusAprobacionSolicitante);
            await _context.SaveChangesAsync();
            return statusAprobacionSolicitante;
        }

        public async Task<StatusAprobacionSolicitante> Modificar(int idStatusAprobacionSolicitante, StatusAprobacionSolicitante statusAprobacionSolicitante)
        {
            _context.Entry(statusAprobacionSolicitante).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return statusAprobacionSolicitante;
        }

        public async Task<StatusAprobacionSolicitante> Eliminar(int idStatusAprobacionSolicitante)
        {
            var statusAprobacionSolicitante = await _context.StatusAprobacionSolicitante.Where(p => p.idStatusAprobacionSolicitante == idStatusAprobacionSolicitante).FirstOrDefaultAsync();
            _context.StatusAprobacionSolicitante.Remove(statusAprobacionSolicitante);
            await _context.SaveChangesAsync();
            return statusAprobacionSolicitante;
        }

        public async Task<bool> StatusAprobacionSolicitanteExiste(int idStatusAprobacionSolicitante)
        {
            return await _context.StatusAprobacionSolicitante.AnyAsync(p => p.idStatusAprobacionSolicitante == idStatusAprobacionSolicitante);
        }

        public async Task<bool> StatusAprobacionSolicitanteExisteRegistro(string descripcionStatusAprobacionSolicitante)
        {
            return await _context.StatusAprobacionSolicitante.AnyAsync(p => p.descripcionStatusAprobacionSolicitante == descripcionStatusAprobacionSolicitante);
        }

    }
}
