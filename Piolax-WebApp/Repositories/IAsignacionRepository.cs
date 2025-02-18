using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IAsignacionRepository
    {
        // CRUD Basico
        Task<Asignaciones> AgregarAsignacion(Asignaciones asignaciones);
        Task<IEnumerable<Asignaciones>> ConsultarTodasLasAsignaciones();
        Task<Asignaciones> ConsultarAsignacionPorId(int idAsignacion);
        Task<Asignaciones> ActualizarAsignacion(int idAsignacion, Asignaciones asignaciones);
        Task<bool> EliminarAsignacion(int idAsignacion);


        // Utilidades
        Task<bool> AsignacionExiste(int idAsignacion);
        Task<IEnumerable<Asignaciones>> ConsultarAsignacionesCompletadas(int idMaquina, int idArea, int? idEmpleado);
       
    }
}
