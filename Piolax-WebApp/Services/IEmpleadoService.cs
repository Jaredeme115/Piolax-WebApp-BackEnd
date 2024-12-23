using Piolax_WebApp.Models;
using Piolax_WebApp.DTOs;

namespace Piolax_WebApp.Services
{
    public interface IEmpleadoService
    {
        Task<Empleado> Consultar(string numNomina);
        Task<IEnumerable<Empleado>> ConsultarTodos();
        Task<EmpleadoInfoDTO> ConsultarEmpleadoConDetalles(string numNomina);
        Task<Empleado> Registro(RegistroDTO registro);
        Task<Empleado> Modificar(string numNomina, RegistroDTO registro);
        Task<Empleado> Eliminar(string numNomina);
        Task<bool> EmpleadoExiste(string numNomina);
        Task<IEnumerable<Empleado>> ConsultarPorStatus(int idStatusEmpleado);
        ResultadoLogin EmpleadoExisteLogin(LoginDTO login);
        Task<Empleado> ConsultarPorId(int idEmpleado);

        //Carga masiva de empleados
        Task<string> RegistrarEmpleadosDesdeExcel(IFormFile filePath);

    }
}
