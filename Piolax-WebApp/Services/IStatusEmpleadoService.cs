using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IStatusEmpleadoService
    {
        Task<StatusEmpleado> Consultar(int idStatusEmpleado);
        Task<IEnumerable<StatusEmpleado>> ConsultarTodos();
        Task<StatusEmpleado> Registro(StatusEmpleadoDTO statusEmpleado);
        Task<StatusEmpleado> Modificar(int idStatusEmpleado, StatusEmpleadoDTO statusEmpleado);
        Task<StatusEmpleado> Eliminar(int idStatusEmpleado);
        Task<bool> StatusEmpleadoExiste(int idStatusEmpleado);
        Task<bool> StatusEmpleadoExisteRegistro(string nombreStatusEmpleado);

    }
}
