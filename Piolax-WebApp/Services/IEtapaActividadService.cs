using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IEtapaActividadService
    {
        Task<IEnumerable<EtapaActividadDTO>> GetAllAsync();
        Task<EtapaActividadDTO?> GetByIdAsync(int id);
        Task<EtapaActividadDTO> CreateAsync(EtapaActividadDTO dto);
        Task<bool> UpdateAsync(int id, EtapaActividadDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
