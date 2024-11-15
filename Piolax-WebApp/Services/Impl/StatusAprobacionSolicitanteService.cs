using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class StatusAprobacionSolicitanteService(IStatusAprobacionSolicitanteRepository repository) : IStatusAprobacionSolicitanteService
    {
        private readonly IStatusAprobacionSolicitanteRepository _repository = repository;

        public Task<IEnumerable<StatusAprobacionSolicitante>> ConsultarTodos()
        {
            return _repository.ConsultarTodos();
        }

        public Task<StatusAprobacionSolicitante> Consultar(int idStatusAprobacionSolicitante)
        {
            return _repository.Consultar(idStatusAprobacionSolicitante);
        }

        public async Task<StatusAprobacionSolicitante> Registro(StatusAprobacionSolicitanteDTO statusAprobacionSolicitante)
        {
            var statusAprobacionSolicitantes = new StatusAprobacionSolicitante
            {
                descripcionStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante,
            };

            return await _repository.Registro(statusAprobacionSolicitantes);
        }

        public async Task<StatusAprobacionSolicitante> Modificar(int idStatusAprobacionSolicitante, StatusAprobacionSolicitanteDTO statusAprobacionSolicitante)
        {
            var statusAprobacionSolicitanteExistente = await _repository.Consultar(idStatusAprobacionSolicitante);

            if (statusAprobacionSolicitanteExistente == null)
                return null; // Devuelve null si el status de aprobacion solicitante no existe

            // Actualizamos los datos del status de aprobacion solicitante
            statusAprobacionSolicitanteExistente.descripcionStatusAprobacionSolicitante = statusAprobacionSolicitante.descripcionStatusAprobacionSolicitante;

            return await _repository.Modificar(idStatusAprobacionSolicitante, statusAprobacionSolicitanteExistente);
        }

        public async Task<StatusAprobacionSolicitante> Eliminar(int idStatusAprobacionSolicitante)
        {
            return await _repository.Eliminar(idStatusAprobacionSolicitante);
        }

        public async Task<bool> StatusAprobacionSolicitanteExiste(int idStatusAprobacionSolicitante)
        {
            return await _repository.StatusAprobacionSolicitanteExiste(idStatusAprobacionSolicitante);
        }

        public async Task<bool> StatusAprobacionSolicitanteExisteRegistro(string descripcionStatusAprobacionSolicitante)
        {
            return await _repository.StatusAprobacionSolicitanteExisteRegistro(descripcionStatusAprobacionSolicitante);
        }
    }
}
