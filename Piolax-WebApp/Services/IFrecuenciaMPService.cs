using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IFrecuenciaMPService
    {
        Task<FrecuenciaMP> ConsultarFrecuenciaPreventivoPorID(int idFrecuenciaPreventivo);
        Task<IEnumerable<FrecuenciaMP>> ConsultarTodasFrecuenciasPreventivo();
    }
}
