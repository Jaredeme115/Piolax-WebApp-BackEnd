using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IEmpleadoAreaRolService
    {
        Task RegistrarEmpleadoConAreaYRol(RegistroDTO registroDTO);

        Task ModificarEmpleadoAreaRol(string numNomina, RegistroDTO registroDTO);

        Task AsignarAreaRol(string numNomina, int idArea, int idRol);

        Task EliminarAreaRol(string numNomina, int idArea, int idRol);

        Task<IEnumerable<EmpleadoAreaRol>> ObtenerAreasRolesPorEmpleado(string numNomina);

        Task<string?> ObtenerRolPorEmpleadoYArea(string numNomina, int idArea);

    }
}
