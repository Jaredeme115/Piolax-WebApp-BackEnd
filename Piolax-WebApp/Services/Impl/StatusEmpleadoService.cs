using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class StatusEmpleadoService(IStatusEmpleadoRepository repository) : IStatusEmpleadoService
    {
        private readonly IStatusEmpleadoRepository _repository = repository;

        public async Task<StatusEmpleado> Consultar(int idStatusEmpleado)
        {
            return await _repository.Consultar(idStatusEmpleado);
        }

        public async Task<IEnumerable<StatusEmpleado>> ConsultarTodos()
        {
            return await _repository.ConsultarTodos();
        }

        public async Task<StatusEmpleado> Registro(StatusEmpleadoDTO statusEmpleado)
        {
            var statusEmpleadoModel = new StatusEmpleado
            {
                descripcionStatusEmpleado = statusEmpleado.descripcionStatusEmpleado
            };

            return await _repository.Registro(statusEmpleadoModel);
        }

        public async Task<StatusEmpleado> Modificar(int idStatusEmpleado, StatusEmpleadoDTO statusEmpleado)
        {
            var statusEmpleadoExistente = await _repository.Consultar(idStatusEmpleado);

            if (statusEmpleadoExistente == null)
                return null; // Devuelve null si el rol no existe

            // Actualizamos los datos del rol
            statusEmpleadoExistente.descripcionStatusEmpleado = statusEmpleado.descripcionStatusEmpleado;

            return await _repository.Modificar(idStatusEmpleado, statusEmpleadoExistente);
        }

        public async Task<StatusEmpleado> Eliminar(int idStatusEmpleado)
        {
            return await _repository.Eliminar(idStatusEmpleado);
        }

        public async Task<bool> StatusEmpleadoExiste(int idStatusEmpleado)
        {
            return await _repository.StatusEmpleadoExiste(idStatusEmpleado);
        }

        public async Task<bool> StatusEmpleadoExisteRegistro(string nombreStatusEmpleado)
        {
            return await _repository.StatusEmpleadoExisteRegistro(nombreStatusEmpleado);
        }



    }
}
