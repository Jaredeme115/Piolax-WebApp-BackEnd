﻿using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IMantenimientoPreventivoRepository
    {
       Task<MantenimientoPreventivo> CrearMantenimientoPreventivo(MantenimientoPreventivo mantenimientoPreventivo);
       Task<MantenimientoPreventivo> ConsultarMP(int idMP);
       Task<MantenimientoPreventivo> Modificar(int idMP, MantenimientoPreventivo mantenimientoPreventivo);
       Task<bool> Eliminar(int idMP);
       Task<IEnumerable<MantenimientoPreventivo>> ConsultarTodosMPs();
       Task<IEnumerable<MantenimientoPreventivo>> MostrarMPsAsignados(int idEmpleado);
       Task<IEnumerable<MantenimientoPreventivo>> ConsultarMantenimientosPorPeriodo(DateTime inicio, DateTime fin);
       Task<MantenimientoPreventivo> ActivarMP(int idMP);
       Task<MantenimientoPreventivo> DesactivarMP(int idMP);
       Task<MantenimientoPreventivo> CambiarEstatusEnProceso(int idMP);
       Task<MantenimientoPreventivo> CancelarMantenimientoEnProceso(int idMP);
    }
}
