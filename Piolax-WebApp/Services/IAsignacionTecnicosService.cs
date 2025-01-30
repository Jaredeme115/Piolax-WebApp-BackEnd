﻿using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface IAsignacionTecnicosService
    {
        Task<IEnumerable<Asignacion_TecnicoDetallesDTO>> ConsultarTecnicosPorAsignacion(int idAsignacion);
        Task<Asignacion_Tecnico> CrearAsignacionTecnico(Asignacion_TecnicoDTO asignacionTecnicoDTO);
        Task<IEnumerable<Asignacion_Tecnico>> ConsultarTodosLosTecnicos();
        Task<bool> EliminarTecnicoDeAsignacion(int idAsignacionTecnico);
        Task<bool> ActualizarTecnicoEnAsignacion(Asignacion_TecnicoDTO asignacionTecnicoDTO);
        Task<Asignacion_Tecnico> FinalizarAsignacionTecnico(Asignacion_TecnicoDTO asignacionTecnicoDTO);
        Task<IEnumerable<Asignacion_TecnicoDetallesDTO>> ConsultarTecnicosConDetallesPorAsignacion(int idAsignacion);
    }
}
