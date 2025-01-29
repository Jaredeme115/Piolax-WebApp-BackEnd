using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IAsignacionRefaccionesService
    {
        Task<IEnumerable<asignacion_refacciones>> ConsultarRefaccionesPorAsignacion(int idAsignacion);
        Task<asignacion_refacciones> CrearAsignacionRefacciones(Asignacion_RefaccionesDTO asignacionRefaccionesDTO);
        Task<IEnumerable<Asignacion_RefaccionesDetallesDTO>> ConsultarTodasLasRefacciones();
        Task<bool> EliminarRefaccionDeAsignacion(int idAsignacionRefaccion);
        Task<bool> ActualizarRefaccionEnAsignacion(Asignacion_RefaccionesDTO asignacionRefaccionesDTO);
        Task<IEnumerable<Asignacion_RefaccionesDetallesDTO>> ConsultarRefaccionesConDetallesPorAsignacion(int idAsignacion);
    }
}
