using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Repositories.Impl;
using System.Security.Cryptography;
using System.Text;

namespace Piolax_WebApp.Services.Impl
{
    public class EmpleadoAreaRolService(IEmpleadoAreaRolRepository repository, IEmpleadoRepository empleadoRepository) : IEmpleadoAreaRolService
    {
        private readonly IEmpleadoAreaRolRepository _repository = repository;
        private readonly IEmpleadoRepository _empleadoRepository = empleadoRepository;

        public async Task RegistrarEmpleadoConAreaYRol(RegistroDTO registroDTO)
        {
            using var hmac = new HMACSHA512();
            var empleado = new Empleado
            {
                numNomina = registroDTO.numNomina,
                nombre = registroDTO.nombre,
                apellidoPaterno = registroDTO.apellidoPaterno,
                apellidoMaterno = registroDTO.apellidoMaterno,
                telefono = registroDTO.telefono,
                email = registroDTO.email,
                fechaIngreso = registroDTO.fechaIngreso,
                idStatusEmpleado = registroDTO.idStatusEmpleado,
                passwordHasH = hmac.ComputeHash(Encoding.UTF8.GetBytes(registroDTO.password)),
                passwordSalt = hmac.Key
            };

            var empleadoAreaRol = new EmpleadoAreaRol
            {
                idEmpleado = empleado.idEmpleado,
                idArea = registroDTO.idArea,
                idRol = registroDTO.idRol,
                esAreaPrincipal = registroDTO.esAreaPrincipal
            };

            await _repository.RegistrarEmpleadoYAsignarAreaRol(empleado, empleadoAreaRol);
        }

        public async Task ModificarEmpleadoAreaRol(string numNomina, RegistroDTO registroDTO)
        {
            var empleadoExistente = await _empleadoRepository.Consultar(numNomina);

            if (empleadoExistente == null)
                throw new Exception("El empleado no existe.");

            // Actualizar los datos del empleado
            empleadoExistente.nombre = registroDTO.nombre;
            empleadoExistente.apellidoPaterno = registroDTO.apellidoPaterno;
            empleadoExistente.apellidoMaterno = registroDTO.apellidoMaterno;
            empleadoExistente.telefono = registroDTO.telefono;
            empleadoExistente.email = registroDTO.email;
            empleadoExistente.fechaIngreso = registroDTO.fechaIngreso;
            //empleadoExistente.idStatusEmpleado = registroDTO.idStatusEmpleado;

            // Si hay un cambio de contraseña
            if (!string.IsNullOrWhiteSpace(registroDTO.password))
            {
                using var hmac = new HMACSHA512();
                empleadoExistente.passwordHasH = hmac.ComputeHash(Encoding.UTF8.GetBytes(registroDTO.password));
                empleadoExistente.passwordSalt = hmac.Key;
            }

            // Obtener las áreas y roles actuales del empleado
            var areasRolesActuales = await _repository.ObtenerAreasRolesPorEmpleado(numNomina);

            // Verificar si el empleado ya tiene un área principal
            var areaPrincipalActual = areasRolesActuales.FirstOrDefault(ar => ar.esAreaPrincipal);

            if (areaPrincipalActual != null)
            {
                // Cambiar el área principal actual a secundaria
                areaPrincipalActual.esAreaPrincipal = false;
                await _repository.ModificarEmpleadoAreaRol(areaPrincipalActual.Empleado, areaPrincipalActual);
            }

            // Verificar si la nueva área ya está en el listado de áreas secundarias
            var nuevaAreaRol = areasRolesActuales.FirstOrDefault(ar => ar.idArea == registroDTO.idArea);

            if (nuevaAreaRol != null)
            {
                // Actualizar el área secundaria a principal
                nuevaAreaRol.esAreaPrincipal = true;
                nuevaAreaRol.idRol = registroDTO.idRol;
                await _repository.ModificarEmpleadoAreaRol(nuevaAreaRol.Empleado, nuevaAreaRol);
            }
            else
            {
                // Agregar la nueva área como principal
                var nuevoEmpleadoAreaRol = new EmpleadoAreaRol
                {
                    idEmpleado = empleadoExistente.idEmpleado,
                    idArea = registroDTO.idArea,
                    idRol = registroDTO.idRol,
                    esAreaPrincipal = true
                };
                await _repository.AgregarAreaYRol(numNomina, registroDTO.idArea, registroDTO.idRol, true);
            }

            // Actualizar los datos del empleado en el repositorio
            await _empleadoRepository.Modificar(empleadoExistente);
        }

        public async Task AsignarAreaRol(string numNomina, int idArea, int idRol, bool esAreaPrincipal)
        {
            await _repository.AgregarAreaYRol(numNomina, idArea, idRol, esAreaPrincipal);
        }

        public async Task<IEnumerable<EmpleadoAreaRol>> ObtenerAreasRolesPorEmpleado(string numNomina)
        {
            // Llama al repositorio para obtener las áreas y roles del empleado
            return await _repository.ObtenerAreasRolesPorEmpleado(numNomina);
        }

        public async Task EliminarAreaRol(string numNomina, int idArea, int idRol)
        {
            await _repository.EliminarAreaYRol(numNomina, idArea, idRol);
        }

        public async Task<IEnumerable<Roles>> ObtenerRolPorEmpleadoYArea(string numNomina, int idArea)
        {
            return await _repository.ObtenerRolPorEmpleadoYArea(numNomina, idArea);
        }

        public async Task<bool> ValidarRolPorEmpleadoYArea(string numNomina, int idArea)
        {
            return await _repository.ValidarRolPorEmpleadoYArea(numNomina, idArea);
        }

        public async Task<bool> TieneAreaPrincipal(string numNomina)
        {
            return await _repository.TieneAreaPrincipal(numNomina);
        }

        public async Task<IEnumerable<Areas>> ObtenerAreaPorEmpleado(string numNomina)
        {
            return await _repository.ObtenerAreaPorEmpleado(numNomina);
        }

        //Metodo para obtener la informacion detallada de todos los empleados (area y rol incluido)
        public async Task<IEnumerable<EmpleadoAreaRol>> ConsultarTodosConDetalles()
        {
            return await _repository.ConsultarTodosConDetalles();
        }

    }
}
