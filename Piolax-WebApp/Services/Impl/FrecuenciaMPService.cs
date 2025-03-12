using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class FrecuenciaMPService(IFrecuenciaMPRepository repository): IFrecuenciaMPService
    {
        private readonly IFrecuenciaMPRepository _repository = repository;

        public async Task<FrecuenciaMP> ConsultarFrecuenciaPreventivoPorID(int idFrecuenciaPreventivo)
        {
            return await _repository.ConsultarFrecuenciaPreventivoPorID(idFrecuenciaPreventivo);
        }

        public async Task<IEnumerable<FrecuenciaMP>> ConsultarTodasFrecuenciasPreventivo()
        {
            return await _repository.ConsultarTodasFrecuenciasPreventivo();
        }
    }
}
