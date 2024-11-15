using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IStatusOrdenService
    {
        Task<StatusOrden> Consultar(int idStatusOrden);
        Task<IEnumerable<StatusOrden>> ConsultarTodos();
        Task<StatusOrden> Registro(StatusOrdenDTO statusOrden);
        Task<StatusOrden> Modificar(int idStatusOrden, StatusOrdenDTO statusOrden);
        Task<StatusOrden> Eliminar(int idStatusOrden);
        Task<bool> StatusOrdenExiste(int idStatusOrden);
        Task<bool> StatusOrdenExisteRegistro(string descripcionStatusOrden);
    }
}
