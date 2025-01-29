using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IAsignacionRefaccionesRepository
    {
        Task<IEnumerable<asignacion_refacciones>> ConsultarRefaccionesPorAsignacion(int idAsignacion);
        Task<asignacion_refacciones> CrearAsignacionRefacciones(asignacion_refacciones asignacionRefacciones);
        Task<IEnumerable<asignacion_refacciones>> ConsultarTodasLasRefacciones();
        Task<bool> EliminarRefaccionDeAsignacion(int idAsignacionRefaccion);
        Task<bool> ActualizarRefaccionEnAsignacion(asignacion_refacciones asignacionRefacciones);
        Task<asignacion_refacciones> ConsultarRefaccionesPorId(int idAsignacionRefaccion);
    }
}
