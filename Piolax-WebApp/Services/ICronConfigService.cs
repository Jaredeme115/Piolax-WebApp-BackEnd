using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface ICronConfigService 
    {
        Task<IEnumerable<CronConfigDTO>> ConsultarTodos();
        Task<CronConfigDTO> Consultar(string nombreCronConfig);
        Task<CronConfigDTO> Actualizar(string nombreCronConfig, UpdateCronConfigDTO updateCronConfigDTO);
    }
}
