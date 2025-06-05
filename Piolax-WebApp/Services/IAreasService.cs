using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IAreasService
    {
        Task<Areas> Consultar(int idArea);
        Task<IEnumerable<Areas>> ConsultarTodos();
        Task<Areas> Registro(AreaDTO area);
        Task<Areas> Modificar(int idArea, AreaDTO area);
        Task<Areas> Eliminar(int idArea);
        Task<bool> AreaExiste(int idArea);
        Task<bool> AreaExisteRegistro(string nombreArea);
        Task RecalcularTodosLosContadores();
    }
}
