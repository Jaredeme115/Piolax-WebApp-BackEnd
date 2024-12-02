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

        //Metodo para obtener el rol de un empleado en un area
        Task<IEnumerable<Roles>> ObtenerRolPorEmpleadoYArea(string numNomina, int idArea);

        //Metodo para validar si empleado tiene un rol en un area
        Task<bool> ValidarRolPorEmpleadoYArea(string numNomina, int idArea);

        //Metodo para obtener el area de un empleado
        Task<IEnumerable<Areas>> ObtenerAreaPorEmpleado(string numNomina);

    }
}
