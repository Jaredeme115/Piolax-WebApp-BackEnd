using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class CategoriaTicketRepository(AppDbContext context) : ICategoriaTicketRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<categoriaTicket?> Consultar(int idCategoriaTicket)
        {
            return await _context.categoriaTicket.Where(p => p.idCategoriaTicket == idCategoriaTicket).FirstOrDefaultAsync();
        }
    }
}
