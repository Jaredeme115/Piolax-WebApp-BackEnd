using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IProyectoService
    {
        Task<IEnumerable<ProyectoDTO>> GetAllAsync();
        Task<ProyectoDTO?> GetByIdAsync(int id);
        Task<ProyectoDTO> CreateAsync(ProyectoDTO dto);
        Task<bool> UpdateAsync(int id, ProyectoDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
