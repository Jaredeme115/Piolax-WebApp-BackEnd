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

            // 🔹 Actualizar solo los campos que NO sean nulos o vacíos en el DTO
            empleadoExistente.nombre = !string.IsNullOrWhiteSpace(registroDTO.nombre) ? registroDTO.nombre : empleadoExistente.nombre;
            empleadoExistente.apellidoPaterno = !string.IsNullOrWhiteSpace(registroDTO.apellidoPaterno) ? registroDTO.apellidoPaterno : empleadoExistente.apellidoPaterno;
            empleadoExistente.apellidoMaterno = !string.IsNullOrWhiteSpace(registroDTO.apellidoMaterno) ? registroDTO.apellidoMaterno : empleadoExistente.apellidoMaterno;
            empleadoExistente.telefono = !string.IsNullOrWhiteSpace(registroDTO.telefono) ? registroDTO.telefono : empleadoExistente.telefono;
            empleadoExistente.email = !string.IsNullOrWhiteSpace(registroDTO.email) ? registroDTO.email : empleadoExistente.email;
            empleadoExistente.fechaIngreso = registroDTO.fechaIngreso != default ? registroDTO.fechaIngreso : empleadoExistente.fechaIngreso;
            empleadoExistente.idStatusEmpleado = registroDTO.idStatusEmpleado != 0 ? registroDTO.idStatusEmpleado : empleadoExistente.idStatusEmpleado;

            // 🔹 Si hay un cambio de contraseña, actualizarla
            if (!string.IsNullOrWhiteSpace(registroDTO.password))
            {
                using var hmac = new HMACSHA512();
                empleadoExistente.passwordHasH = hmac.ComputeHash(Encoding.UTF8.GetBytes(registroDTO.password));
                empleadoExistente.passwordSalt = hmac.Key;
            }

            // 🔹 Obtener todas las áreas actuales del empleado
            var areasRolesActuales = await _repository.ObtenerAreasRolesPorEmpleado(numNomina);
            var areaPrincipalActual = areasRolesActuales.FirstOrDefault(ar => ar.esAreaPrincipal);

            if (registroDTO.idArea != 0 && registroDTO.idRol != 0)
            {
                // 🔹 Eliminar completamente el área y rol anterior
                if (areaPrincipalActual != null)
                {
                    await _repository.EliminarAreaYRol(numNomina, areaPrincipalActual.idArea, areaPrincipalActual.idRol);
                }

                // 🔹 Verificar si la nueva área y rol ya están asignados al empleado
                var nuevaAreaRol = areasRolesActuales.FirstOrDefault(ar => ar.idArea == registroDTO.idArea && ar.idRol == registroDTO.idRol);

                if (nuevaAreaRol == null)
                {
                    // ✅ Si la nueva área y rol NO existen, agregarlos como principal
                    var nuevoEmpleadoAreaRol = new EmpleadoAreaRol
                    {
                        idEmpleado = empleadoExistente.idEmpleado,
                        idArea = registroDTO.idArea,
                        idRol = registroDTO.idRol,
                        esAreaPrincipal = true
                    };
                    await _repository.AgregarAreaYRol(numNomina, registroDTO.idArea, registroDTO.idRol, true);
                }
            }

            // 🔹 Guardar cambios en el empleado
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

        public async Task<IEnumerable<Roles>> ObtenerRolesPorEmpleado(string numNomina)
        {
            return await _repository.ObtenerRolesPorEmpleado(numNomina);
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

        public async Task<IEnumerable<EmpleadoNombreCompletoDTO>> ObtenerEmpleadosPorArea(int idArea)
        {
            return await _repository.ObtenerEmpleadosPorArea(idArea);
        }


    }
}
