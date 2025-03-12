using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class EstatusPreventivoService(IEstatusPreventivoRepository repository): IEstatusPreventivoService
    {
        private readonly IEstatusPreventivoRepository _repository = repository;

        public async Task<EstatusPreventivo> ConsultarEstatusPreventivoPorID(int idEstatusPreventivo)
        {
            return await _repository.ConsultarEstatusPreventivoPorID(idEstatusPreventivo);
        }

        public async Task<IEnumerable<EstatusPreventivo>> ConsultarTodosEstatusPreventivo()
        {
            return await _repository.ConsultarTodosEstatusPreventivo();
        }
    }
}
