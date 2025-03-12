using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class FrecuenciaMPRepository(AppDbContext context) : IFrecuenciaMPRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<FrecuenciaMP> ConsultarFrecuenciaPreventivoPorID(int idFrecuenciaPreventivo)
        {
            return await _context.FrecuenciaMP.Where(p => p.idFrecuenciaPreventivo == idFrecuenciaPreventivo).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FrecuenciaMP>> ConsultarTodasFrecuenciasPreventivo()
        {
            return await _context.FrecuenciaMP.ToListAsync();
        }

    }
}
