using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class MaquinasService(IMaquinasRepository repository) : IMaquinasService
    {
        private readonly IMaquinasRepository _repository = repository;

        public Task<IEnumerable<Maquinas>> ConsultarTodos()
        {
            return _repository.ConsultarTodos();
        }

        public Task<Maquinas> Consultar(int idMaquina)
        {
            return _repository.Consultar(idMaquina);
        }

        public async Task<Maquinas> Registro(MaquinaDTO maquina)
        {
            var maquinas = new Maquinas
            {
                nombreMaquina = maquina.descripcion,
                codigoQR = maquina.codigoQR,
                idArea = maquina.idArea
            };

            return await _repository.Registro(maquinas);
        }

        public async Task<Maquinas> Modificar(int idMaquina, MaquinaDTO maquina)
        {
            var maquinaExistente = await _repository.Consultar(idMaquina);

            if (maquinaExistente == null)
                return null; // Devuelve null si la maquina no existe

            // Actualizamos los datos de la maquina
            maquinaExistente.nombreMaquina = maquina.descripcion;

            return await _repository.Modificar(idMaquina, maquinaExistente);
        }

        public async Task<Maquinas> Eliminar(int idMaquina)
        {
            return await _repository.Eliminar(idMaquina);
        }

        public async Task<bool> MaquinaExiste(int idMaquina)
        {
            return await _repository.MaquinaExiste(idMaquina);
        }

        public async Task<bool> MaquinaExisteRegistro(string nombreMaquina)
        {
            return await _repository.MaquinaExisteRegistro(nombreMaquina);
        }

    }
}
