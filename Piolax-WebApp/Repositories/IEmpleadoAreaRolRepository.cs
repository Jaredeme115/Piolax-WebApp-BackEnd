using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IEmpleadoAreaRolRepository
    {
        Task RegistrarEmpleadoYAsignarAreaRol(Empleado empleado, EmpleadoAreaRol empleadoAreaRol);

        Task ModificarEmpleadoAreaRol(Empleado empleado, EmpleadoAreaRol empleadoAreaRol);

        Task AgregarAreaYRol(string numNomina, int idArea, int idRol);

        Task<IEnumerable<EmpleadoAreaRol>> ObtenerAreasRolesPorEmpleado(string numNomina);
    }
}
