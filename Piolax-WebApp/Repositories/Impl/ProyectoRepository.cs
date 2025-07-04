using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class ProyectoRepository: IProyectoRepository
    {
        private readonly AppDbContext _context;
        public ProyectoRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Proyecto>> GetAllAsync() =>
            await _context.Proyecto.ToListAsync();

        public async Task<Proyecto?> GetByIdAsync(int id) =>
            await _context.Proyecto.FindAsync(id);

        public async Task AddAsync(Proyecto entity) =>
            await _context.Proyecto.AddAsync(entity);

        public void Update(Proyecto entity) =>
            _context.Proyecto.Update(entity);

        public void Remove(Proyecto entity) =>
            _context.Proyecto.Remove(entity);

        public async Task<bool> SaveChangesAsync() =>
            (await _context.SaveChangesAsync()) > 0;
    }
}
