using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface ICronConfigRepository
    {
        Task<string> ObtenerHorarioPorNombre(string nombreCronConfig);
        Task<IEnumerable<CronConfig>> GetAll();
        Task ActualizarHora(string nombreCronConfig, string horaCron);
    }
}

