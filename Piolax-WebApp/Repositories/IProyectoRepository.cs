using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IProyectoRepository
    {
        Task<IEnumerable<Proyecto>> GetAllAsync();
        Task<Proyecto?> GetByIdAsync(int id);
        Task AddAsync(Proyecto entity);
        void Update(Proyecto entity);
        void Remove(Proyecto entity);
        Task<bool> SaveChangesAsync();
    }
}
