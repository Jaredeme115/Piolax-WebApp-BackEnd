using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IAreasRepository
    {
        Task<Areas> Consultar(int idArea);
        Task<IEnumerable<Areas>> ConsultarTodos();
        Task<Areas> Registro(Areas area);
        Task<Areas> Modificar(int idArea, Areas area);
        Task<Areas> Eliminar(int idArea);
        Task<bool> AreaExiste(int idArea);
        Task<bool> AreaExisteRegistro(string nombreArea);
        Task ActualizarContadorMaquinasActivas(int idArea);
        Task RecalcularTodosLosContadores();
    }
}
