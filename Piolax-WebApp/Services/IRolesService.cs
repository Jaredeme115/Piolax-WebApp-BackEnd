using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IRolesService
    {
        Task<Roles> Consultar(int idRol);
        Task<IEnumerable<Roles>> ConsultarTodos();
        Task<Roles> Registro(RolDTO rol);
        Task<Roles> Modificar(int idRol, RolDTO rol);
        Task<Roles> Eliminar(int idRol);
        Task<bool> RolExiste(int idRol);
        Task<bool> RolExisteRegistro(string nombreRol);
    }
}
