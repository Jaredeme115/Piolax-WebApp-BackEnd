using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class EtapaComentarioRepository: IEtapaComentarioRepository
    {
        private readonly AppDbContext _context;
        public EtapaComentarioRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<EtapaComentario>> GetAllAsync() =>
            await _context.EtapaComentario.ToListAsync();

        public async Task<EtapaComentario?> GetByIdAsync(int id) =>
            await _context.EtapaComentario.FindAsync(id);

        public async Task AddAsync(EtapaComentario entity) =>
            await _context.EtapaComentario.AddAsync(entity);

        public void Update(EtapaComentario entity) =>
            _context.EtapaComentario.Update(entity);

        public void Remove(EtapaComentario entity) =>
            _context.EtapaComentario.Remove(entity);

        public async Task<bool> SaveChangesAsync() =>
            (await _context.SaveChangesAsync()) > 0;
    }
}
