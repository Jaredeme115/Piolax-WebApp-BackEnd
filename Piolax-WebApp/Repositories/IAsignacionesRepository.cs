﻿using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IAsignacionesRepository
    {
        Task<Asignaciones> RegistrarAsignacion(Asignaciones asignaciones);
        Task<Asignaciones> ObtenerAsignacionConDetalles(int idAsignacion);
        Task<IEnumerable<Asignaciones>> ObtenerTodasLasAsignaciones();
        Task<IEnumerable<Asignaciones>> ObtenerAsignacionPorTecnico(string numNomina);
        Task<AsignacionesDetalleDTO> ModificarEstatusAprobacionTecnico(int idAsignacion, int idStatusAprobacionTecnico);
    }
}
