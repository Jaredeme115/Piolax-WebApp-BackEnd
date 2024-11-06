using Piolax_WebApp.Models;
using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IEmpleadoService
    {
        Task<Empleado> Consultar(string numNomina);
        Task<IEnumerable<Empleado>> ConsultarTodos();
        Task<Empleado> Registro(RegistroDTO registro);
        Task<bool> EmpleadoExiste(string numNomina);
        ResultadoLogin EmpleadoExisteLogin(LoginDTO login);
    }
}
