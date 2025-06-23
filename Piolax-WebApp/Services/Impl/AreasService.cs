using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class AreasService(IAreasRepository repository) : IAreasService
    {
        private readonly IAreasRepository _repository = repository;

        public Task<IEnumerable<Areas>> ConsultarTodos()
        {
            return _repository.ConsultarTodos();
        }

        public Task<Areas> Consultar(int idArea)
        {
            return _repository.Consultar(idArea);
        }

        public async Task<Areas> Registro(AreaDTO area)
        {
            var areas = new Areas
            {
                nombreArea = area.nombreArea
            };

            return await _repository.Registro(areas);
        }

        public async Task<Areas> Modificar(int idArea, AreaDTO area)
        {
            var areaExistente = await _repository.Consultar(idArea);

            if (areaExistente == null)
                return null; // Devuelve null si el area no existe

            // Actualizamos los datos del area
            areaExistente.nombreArea = area.nombreArea;

            return await _repository.Modificar(idArea, areaExistente);
        }

        public async Task<Areas> Eliminar(int idArea)
        {
            return await _repository.Eliminar(idArea);
        }

        public async Task<bool> AreaExiste(int idArea)
        {
            return await _repository.AreaExiste(idArea);
        }

        public async Task<bool> AreaExisteRegistro(string nombreArea)
        {
            return await _repository.AreaExisteRegistro(nombreArea);
        }

        public async Task RecalcularTodosLosContadores()
        {
            await _repository.RecalcularTodosLosContadores();
        }



    }
}
