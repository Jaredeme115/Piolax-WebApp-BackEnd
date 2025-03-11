using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class EstatusPreventivoRepository(AppDbContext context) : IEstatusPreventivoRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<EstatusPreventivo> ConsultarEstatusPreventivoPorID(int idEstatusPreventivo)
        {
            return await _context.EstatusPreventivo.Where(p => p.idEstatusPreventivo == idEstatusPreventivo).FirstOrDefaultAsync();
        }
    }
}
