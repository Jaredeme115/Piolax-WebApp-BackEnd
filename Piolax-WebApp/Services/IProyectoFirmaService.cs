using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IProyectoFirmaService
    {
        Task<IEnumerable<ProyectoFirmaDTO>> GetAllAsync();
        Task<ProyectoFirmaDTO?> GetByIdAsync(int id);
        Task<ProyectoFirmaDTO> CreateAsync(ProyectoFirmaDTO dto);
        Task<bool> UpdateAsync(int id, ProyectoFirmaDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
