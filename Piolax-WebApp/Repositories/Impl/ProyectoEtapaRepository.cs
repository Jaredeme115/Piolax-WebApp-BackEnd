using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class ProyectoEtapaRepository: IProyectoEtapaRepository
    {
        private readonly AppDbContext _context;
        public ProyectoEtapaRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<ProyectoEtapa>> GetAllAsync() =>
            await _context.ProyectoEtapa.ToListAsync();

        public async Task<ProyectoEtapa?> GetByIdAsync(int id) =>
            await _context.ProyectoEtapa.FindAsync(id);

        public async Task AddAsync(ProyectoEtapa entity) =>
            await _context.ProyectoEtapa.AddAsync(entity);

        public void Update(ProyectoEtapa entity) =>
            _context.ProyectoEtapa.Update(entity);

        public void Remove(ProyectoEtapa entity) =>
            _context.ProyectoEtapa.Remove(entity);

        public async Task<bool> SaveChangesAsync() =>
            (await _context.SaveChangesAsync()) > 0;
    }
}
