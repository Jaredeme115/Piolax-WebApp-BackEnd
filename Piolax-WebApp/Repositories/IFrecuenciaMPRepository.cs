using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IFrecuenciaMPRepository
    {
        Task<FrecuenciaMP> ConsultarFrecuenciaPreventivoPorID(int idFrecuenciaMP);
        Task<IEnumerable<FrecuenciaMP>> ConsultarTodasFrecuenciasPreventivo();
    }
}
