using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IEtapaActividadRepository
    {
        Task<IEnumerable<EtapaActividad>> GetAllAsync();
        Task<EtapaActividad?> GetByIdAsync(int id);
        Task AddAsync(EtapaActividad entity);
        void Update(EtapaActividad entity);
        void Remove(EtapaActividad entity);
        Task<bool> SaveChangesAsync();
    }
}
