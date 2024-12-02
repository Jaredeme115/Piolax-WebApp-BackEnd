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
                idRol = registroDTO.idRol
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
            empleadoExistente.idStatusEmpleado = registroDTO.idStatusEmpleado;

            // Si hay un cambio de contraseña
            if (!string.IsNullOrWhiteSpace(registroDTO.password))
            {
                using var hmac = new HMACSHA512();
                empleadoExistente.passwordHasH = hmac.ComputeHash(Encoding.UTF8.GetBytes(registroDTO.password));
                empleadoExistente.passwordSalt = hmac.Key;
            }

            // Crear el nuevo área y rol
            var empleadoAreaRol = new EmpleadoAreaRol
            {
                idEmpleado = empleadoExistente.idEmpleado,
                idArea = registroDTO.idArea,
                idRol = registroDTO.idRol
            };

            // Actualizar en el repositorio
            await _repository.ModificarEmpleadoAreaRol(empleadoExistente, empleadoAreaRol);
        }

        public async Task AsignarAreaRol(string numNomina, int idArea, int idRol)
        {
            await _repository.AgregarAreaYRol(numNomina, idArea, idRol);
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

        public async Task<IEnumerable<Areas>> ObtenerAreaPorEmpleado(string numNomina)
        {
            return await _repository.ObtenerAreaPorEmpleado(numNomina);
        }

    }
}
