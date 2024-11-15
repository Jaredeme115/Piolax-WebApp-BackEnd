using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IStatusOrdenRepository
    {
        Task<StatusOrden> Consultar(int idStatusOrden);
        Task<IEnumerable<StatusOrden>> ConsultarTodos();
        Task<StatusOrden> Registro(StatusOrden statusOrden);
        Task<StatusOrden> Modificar(int idStatusOrden, StatusOrden statusOrden);
        Task<StatusOrden> Eliminar(int idStatusOrden);
        Task<bool> StatusOrdenExiste(int idStatusOrden);
        Task<bool> StatusOrdenExisteRegistro(string descripcionStatusOrden);
    }
}
