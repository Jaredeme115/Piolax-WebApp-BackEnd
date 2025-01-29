using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class StatusAsignacionService (IStatusAsignacionRepository repository) : IStatusAsignacionService
    {
        private readonly IStatusAsignacionRepository _repository = repository;

        public Task<StatusAsignacion> Consultar(int idStatusAsignacion)
        {
            return _repository.Consultar(idStatusAsignacion);
        }
    }
}
