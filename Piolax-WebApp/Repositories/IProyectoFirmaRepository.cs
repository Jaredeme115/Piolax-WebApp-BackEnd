using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IProyectoFirmaRepository
    {
        Task<IEnumerable<ProyectoFirma>> GetAllAsync();
        Task<ProyectoFirma?> GetByIdAsync(int id);
        Task AddAsync(ProyectoFirma entity);
        void Update(ProyectoFirma entity);
        void Remove(ProyectoFirma entity);
        Task<bool> SaveChangesAsync();
    }
}
