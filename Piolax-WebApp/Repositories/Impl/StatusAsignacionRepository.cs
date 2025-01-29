using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class StatusAsignacionRepository(AppDbContext context) : IStatusAsignacionRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<StatusAsignacion?> Consultar(int idStatusAsignacion)
        {
            return await _context.StatusAsignacion.Where(p => p.idStatusAsignacion == idStatusAsignacion).FirstOrDefaultAsync();
        }
    }
}
