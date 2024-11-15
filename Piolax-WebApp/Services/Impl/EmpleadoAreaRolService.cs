using Microsoft.AspNetCore.Identity;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Piolax_WebApp.Services.Impl
{
    public class EmpleadoAreaRolService(IEmpleadoAreaRolRepository repository) : IEmpleadoAreaRolService
    {
        private readonly IEmpleadoAreaRolRepository _repository = repository;

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
    }
}
