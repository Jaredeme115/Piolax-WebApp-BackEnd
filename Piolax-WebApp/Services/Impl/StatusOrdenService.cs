using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class StatusOrdenService(IStatusOrdenRepository repository) : IStatusOrdenService
    {
        private readonly IStatusOrdenRepository _repository = repository;

        public Task<IEnumerable<StatusOrden>> ConsultarTodos()
        {
            return _repository.ConsultarTodos();
        }

        public Task<StatusOrden> Consultar(int idStatusOrden)
        {
            return _repository.Consultar(idStatusOrden);
        }

        public async Task<StatusOrden> Registro(StatusOrdenDTO statusOrden)
        {
            var statusOrdenes = new StatusOrden
            {
                descripcionStatusOrden = statusOrden.descripcionStatusOrden,
                color = statusOrden.color
            };

            return await _repository.Registro(statusOrdenes);
        }

        public async Task<StatusOrden> Modificar(int idStatusOrden, StatusOrdenDTO statusOrden)
        {
            var statusOrdenExistente = await _repository.Consultar(idStatusOrden);

            if (statusOrdenExistente == null)
                return null; // Devuelve null si el status de orden no existe

            // Actualizamos los datos del status de orden
            statusOrdenExistente.descripcionStatusOrden = statusOrden.descripcionStatusOrden;
            statusOrdenExistente.color = statusOrden.color;

            return await _repository.Modificar(idStatusOrden, statusOrdenExistente);
        }

        public async Task<StatusOrden> Eliminar(int idStatusOrden)
        {
            return await _repository.Eliminar(idStatusOrden);
        }

        public async Task<bool> StatusOrdenExiste(int idStatusOrden)
        {
            return await _repository.StatusOrdenExiste(idStatusOrden);
        }

        public async Task<bool> StatusOrdenExisteRegistro(string descripcionStatusOrden)
        {
            return await _repository.StatusOrdenExisteRegistro(descripcionStatusOrden);
        }
    }
}
