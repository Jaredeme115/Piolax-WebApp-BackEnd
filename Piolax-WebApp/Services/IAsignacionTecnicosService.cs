using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IAsignacionTecnicosService
    {
        Task<IEnumerable<Asignacion_TecnicoDetallesDTO>> ConsultarTecnicosPorAsignacion(int idAsignacion);
        Task<Asignacion_TecnicoResponseDTO?> CrearAsignacionTecnico(Asignacion_TecnicoDTO asignacionTecnicoDTO);
        Task<IEnumerable<Asignacion_Tecnico>> ConsultarTodosLosTecnicos();
        Task<bool> EliminarTecnicoDeAsignacion(int idAsignacionTecnico);
        Task<bool> ActualizarTecnicoEnAsignacion(Asignacion_TecnicoDTO asignacionTecnicoDTO);
        Task<Asignacion_TecnicoFinalizacionResponseDTO> FinalizarAsignacionTecnico(Asignacion_TecnicoFinalizacionDTO asignacionTecnicoFinalizacionDTO);
        Task<bool> PausarAsignacion(int idAsignacion, int idTecnicoQuePausa, string comentarioPausa);
        Task<bool> RetirarTecnicoDeApoyo(int idAsignacion, int idTecnicoQueSeRetira, string comentarioRetiro);
        Task<IEnumerable<Asignacion_TecnicoDetallesDTO>> ConsultarTecnicosConDetallesPorAsignacion(int idAsignacion);
        Task<IEnumerable<Asignacion_TecnicoDetallesDTO>> ConsultarOrdenesEnPausaPorTecnico(int idEmpleado);


    }
}
