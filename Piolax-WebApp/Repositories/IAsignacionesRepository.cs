using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IAsignacionesRepository
    {
        // CRUD Basico
        Task<Asignaciones> AgregarAsignacion(Asignaciones asignaciones);
        Task<IEnumerable<Asignaciones>> ConsultarTodasLasAsignaciones();
        Task<Asignaciones> ConsultarAsignacionPorId(int idAsignacion);
        Task<Asignaciones> ActualizarAsignacion(Asignaciones asignaciones);
        Task<bool> EliminarAsignacion(int idAsignacion);


        // Utilidades
        Task<bool> AsignacionExiste(int idAsignacion);
    }
}
