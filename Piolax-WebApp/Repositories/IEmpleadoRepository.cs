﻿using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface IEmpleadoRepository
    {
        Task<Empleado> Consultar(string numNomina);
        Task<IEnumerable<Empleado>> ConsultarTodos();
        Task<bool> EmpleadoExiste(string numNomina);
        Empleado EmpleadoExisteLogin(string numNomina);
        Task<Empleado> Agregar(Empleado empleado);
    }
}
