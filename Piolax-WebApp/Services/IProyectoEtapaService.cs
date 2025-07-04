using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IProyectoEtapaService
    {
        Task<IEnumerable<ProyectoEtapaDTO>> GetAllAsync();
        Task<ProyectoEtapaDTO?> GetByIdAsync(int id);
        Task<ProyectoEtapaDTO> CreateAsync(ProyectoEtapaDTO dto);
        Task<bool> UpdateAsync(int id, ProyectoEtapaDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
