﻿using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IAsignacionTecnicoRepository
    {
        Task<IEnumerable<Asignacion_Tecnico>> ConsultarTecnicosPorAsignacion(int idAsignacion);
        Task<Asignacion_Tecnico> ConsultarTecnicoPorID(int idAsignacion);
        Task<Asignacion_Tecnico> CrearAsignacionTecnico(Asignacion_Tecnico asignacionTecnico);
        Task<IEnumerable<Asignacion_Tecnico>> ConsultarTodosLosTecnicos();
        Task<bool> EliminarTecnicoDeAsignacion(int idAsignacionTecnico);
        Task<bool> ActualizarTecnicoEnAsignacion(Asignacion_Tecnico asignacionTecnico);
        Task<bool> ConsultarTecnicosActivosPorAsignacion(int idAsignacion);


    }
}
