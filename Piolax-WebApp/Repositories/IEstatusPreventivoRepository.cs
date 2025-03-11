using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IEstatusPreventivoRepository
    {
        Task<EstatusPreventivo> ConsultarEstatusPreventivoPorID(int idEstatusPreventivo);
    }
}
