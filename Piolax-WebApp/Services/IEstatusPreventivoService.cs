using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IEstatusPreventivoService
    {
        Task<EstatusPreventivo> ConsultarEstatusPreventivoPorID(int idEstatusPreventivo);
        Task<IEnumerable<EstatusPreventivo>> ConsultarTodosEstatusPreventivo();
    }
}
