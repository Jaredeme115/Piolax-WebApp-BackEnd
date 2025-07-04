using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IProyectoEtapaRepository
    {
        Task<IEnumerable<ProyectoEtapa>> GetAllAsync();
        Task<ProyectoEtapa?> GetByIdAsync(int id);
        Task AddAsync(ProyectoEtapa entity);
        void Update(ProyectoEtapa entity);
        void Remove(ProyectoEtapa entity);
        Task<bool> SaveChangesAsync();
    }
}
