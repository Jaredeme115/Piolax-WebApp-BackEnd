using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IAsignacionTecnicosRepository
    {
        Task<IEnumerable<Asignacion_Tecnico>> ConsultarTecnicosPorAsignacion(int idAsignacion);
        //Task<IEnumerable<Asignacion_Tecnico>> ConsultarTecnicosPorAsignacion(int idAsignacion, bool incluirRetirados = false);
        Task<Asignacion_Tecnico> ConsultarTecnicoPorID(int idAsignacion);
        Task<Asignacion_Tecnico?> ConsultarTecnicoPorAsignacionYEmpleado(int idAsignacion, int idEmpleado);
        Task<Asignacion_Tecnico> CrearAsignacionTecnico(Asignacion_Tecnico asignacionTecnico);
        Task<IEnumerable<Asignacion_Tecnico>> ConsultarTodosLosTecnicos();
        Task<bool> EliminarTecnicoDeAsignacion(int idAsignacionTecnico);
        Task<bool> ActualizarTecnicoEnAsignacion(Asignacion_Tecnico asignacionTecnico);
        Task<bool> ConsultarTecnicosActivosPorAsignacion(int idAsignacion);
        Task<IEnumerable<Asignaciones>> ObtenerAsignacionesPausadasPorTecnico(int idTecnico);
        Task<bool> RetomarAsignacion(int idAsignacion, int idEmpleado);

    }

}

