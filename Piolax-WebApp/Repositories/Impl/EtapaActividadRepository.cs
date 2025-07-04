using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class EtapaActividadRepository: IEtapaActividadRepository
    {
        private readonly AppDbContext _context;
        public EtapaActividadRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<EtapaActividad>> GetAllAsync() =>
            await _context.EtapaActividad.ToListAsync();

        public async Task<EtapaActividad?> GetByIdAsync(int id) =>
            await _context.EtapaActividad.FindAsync(id);

        public async Task AddAsync(EtapaActividad entity) =>
            await _context.EtapaActividad.AddAsync(entity);

        public void Update(EtapaActividad entity) =>
            _context.EtapaActividad.Update(entity);

        public void Remove(EtapaActividad entity) =>
            _context.EtapaActividad.Remove(entity);

        public async Task<bool> SaveChangesAsync() =>
            (await _context.SaveChangesAsync()) > 0;
    }
}
