using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IEtapaComentarioService
    {
        Task<IEnumerable<EtapaComentarioDTO>> GetAllAsync();
        Task<EtapaComentarioDTO?> GetByIdAsync(int id);
        Task<EtapaComentarioDTO> CreateAsync(EtapaComentarioDTO dto);
        Task<bool> UpdateAsync(int id, EtapaComentarioDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
