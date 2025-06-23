using Piolax_WebApp.DTOs;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class CronConfigService (ICronConfigRepository repository): ICronConfigService
    {
        private readonly ICronConfigRepository _repository = repository;

        public async Task<IEnumerable<CronConfigDTO>> ConsultarTodos()
        {
            var entidades = await _repository.GetAll();
            return entidades.Select(c => new CronConfigDTO
            {
                nombreCronConfig = c.nombreCronConfig,
                horaCron = c.horaCron,
                descripcionCron = c.descripcionCron
            });
        }

        public async Task<CronConfigDTO> Consultar(string nombreCronConfig)
        {
            // Usamos el repo para traer la expresión y luego mapeamos
            var expr = await _repository.ObtenerHorarioPorNombre(nombreCronConfig);
            // Si quieres más campos, podrías traer toda la entidad con otro método
            return new CronConfigDTO
            {
                nombreCronConfig = nombreCronConfig,
                horaCron = expr
            };
        }

        public async Task<CronConfigDTO> Actualizar(string nombreCronConfig, UpdateCronConfigDTO updateCronConfigDTO)
        {
            await _repository.ActualizarHora(nombreCronConfig, updateCronConfigDTO.horaCron);

            return new CronConfigDTO
            {
                nombreCronConfig = nombreCronConfig,
                horaCron = updateCronConfigDTO.horaCron
            };
        }
    }
}
