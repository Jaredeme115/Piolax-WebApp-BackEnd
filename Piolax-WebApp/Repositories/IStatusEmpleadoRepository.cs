using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IStatusEmpleadoRepository
    {
        Task<StatusEmpleado> Consultar(int idStatusEmpleado);
        Task<IEnumerable<StatusEmpleado>> ConsultarTodos();
        Task<StatusEmpleado> Registro(StatusEmpleado statusEmpleado);
        Task<StatusEmpleado> Modificar(int idStatusEmpleado, StatusEmpleado statusEmpleado);
        Task<StatusEmpleado> Eliminar(int idStatusEmpleado);
        Task<bool> StatusEmpleadoExiste(int idStatusEmpleado);
        Task<bool> StatusEmpleadoExisteRegistro(string descripcionStatusEmpleado);

    }
}
