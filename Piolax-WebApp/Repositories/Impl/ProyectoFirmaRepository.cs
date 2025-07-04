using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class ProyectoFirmaRepository: IProyectoFirmaRepository
    {
        private readonly AppDbContext _context;
        public ProyectoFirmaRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<ProyectoFirma>> GetAllAsync() =>
            await _context.ProyectoFirma.ToListAsync();

        public async Task<ProyectoFirma?> GetByIdAsync(int id) =>
            await _context.ProyectoFirma.FindAsync(id);

        public async Task AddAsync(ProyectoFirma entity) =>
            await _context.ProyectoFirma.AddAsync(entity);

        public void Update(ProyectoFirma entity) =>
            _context.ProyectoFirma.Update(entity);

        public void Remove(ProyectoFirma entity) =>
            _context.ProyectoFirma.Remove(entity);

        public async Task<bool> SaveChangesAsync() =>
            (await _context.SaveChangesAsync()) > 0;
    }
}
