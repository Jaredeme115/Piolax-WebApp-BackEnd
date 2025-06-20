using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IAsignacionService
    {
        Task<AsignacionResponseDTO> AgregarAsignacion(AsignacionesDTO asignacionesDTO);
        Task<IEnumerable<Asignaciones>> ConsultarTodasLasAsignaciones();
        Task<Asignaciones> ConsultarAsignacionPorId(int idAsignacion);
        Task<Asignaciones> ActualizarAsignacion(int idAsignacion, AsignacionesDTO asignacionesDTO);
        Task<bool> EliminarAsignacion(int idAsignacion);
        Task<AsignacionDetallesDTO?> ConsultarAsignacionConDetallesPorId(int idAsignacion);
        Task GuardarKPIs(int idMaquina, int idArea, int? idEmpleado = null);

        // Métodos de apoyo para MTTA
        Task<double> CalcularMTTA(int idMaquina, int idArea);

        // Métodos de apoyo para MTTR
        Task<double> CalcularMTTR(int idMaquina, int idArea, int? idEmpleado = null);

        // Métodos de apoyo para MTBF
        Task<double> CalcularMTBF(int idArea, int anio, int mes);

    }
}
