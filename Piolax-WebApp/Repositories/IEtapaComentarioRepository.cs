using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IEtapaComentarioRepository
    {
        Task<IEnumerable<EtapaComentario>> GetAllAsync();
        Task<EtapaComentario?> GetByIdAsync(int id);
        Task AddAsync(EtapaComentario entity);
        void Update(EtapaComentario entity);
        void Remove(EtapaComentario entity);
        Task<bool> SaveChangesAsync();
    }
}
