using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IRolesRepository
    {
        Task<Roles> Consultar(int idRol);
        Task<IEnumerable<Roles>> ConsultarTodos();
        Task<Roles> Registro(Roles rol);
        Task<Roles> Modificar(int idRol, Roles rol);
        Task<Roles> Eliminar(int idRol);
        Task<bool> RolExiste(int idRol);
        Task<bool> RolExisteRegistro(string nombreRol);
    }
}
