using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class RolesService(IRolesRepository repository) : IRolesService
    {
        private readonly IRolesRepository _repository = repository;

        public Task<IEnumerable<Roles>> ConsultarTodos()
        {
            return _repository.ConsultarTodos();
        }

        public Task<Roles> Consultar(int idRol)
        {
            return _repository.Consultar(idRol);
        }

        public async Task<Roles> Registro(RolDTO rol)
        {
            var roles = new Roles
            {
                nombreRol = rol.nombreRol
            };
            return await _repository.Registro(roles);
        }

        public async Task<Roles> Modificar(int idRol, RolDTO rol)
        {
            var rolExistente = await _repository.Consultar(idRol);

            if (rolExistente == null)
                return null; // Devuelve null si el rol no existe

            // Actualizamos los datos del rol
            rolExistente.nombreRol = rol.nombreRol;

            return await _repository.Modificar(idRol, rolExistente);
        }

        public async Task<Roles> Eliminar(int idRol)
        {
            return await _repository.Eliminar(idRol);
        }

        public async Task<bool> RolExiste(int idRol)
        {
            return await _repository.RolExiste(idRol);
        }

        public async Task<bool> RolExisteRegistro(string nombreRol)
        {
            return await _repository.RolExisteRegistro(nombreRol);
        }


    }
}
