using System.Security.Cryptography;
using System.Text;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Piolax_WebApp.Repositories.Impl;
using Microsoft.Win32;

namespace Piolax_WebApp.Services.Impl
{
    public class EmpleadoService(IEmpleadoRepository repository, IEmpleadoAreaRolRepository empleadoAreaRolRepository) : IEmpleadoService
    {
        private readonly IEmpleadoRepository _repository = repository;
        private readonly IEmpleadoAreaRolRepository _empleadoAreaRolRepository = empleadoAreaRolRepository;


        public Task<IEnumerable<Empleado>> ConsultarTodos()
        {
            return _repository.ConsultarTodos();
        }

        public async Task<Empleado> Registro(RegistroDTO registroDTO)
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

            return await _repository.Agregar(empleado);
        }

        public async Task<EmpleadoInfoDTO> ConsultarEmpleadoConDetalles(string numNomina)
        {
            return await _empleadoAreaRolRepository.ConsultarEmpleadoConDetalles(numNomina);
        }


        public async Task<Empleado?> Modificar(string numNomina, RegistroDTO registroDTO)
        {
            var empleadoExistente = await _repository.Consultar(numNomina);

            if (empleadoExistente == null)
                return null; // Devuelve null si el empleado no existe

            // Actualizamos los datos del empleado
            empleadoExistente.numNomina = registroDTO.numNomina;
            empleadoExistente.nombre = registroDTO.nombre;
            empleadoExistente.apellidoPaterno = registroDTO.apellidoPaterno;
            empleadoExistente.apellidoMaterno = registroDTO.apellidoMaterno;
            empleadoExistente.telefono = registroDTO.telefono;
            empleadoExistente.email = registroDTO.email;
            empleadoExistente.fechaIngreso = registroDTO.fechaIngreso;
            //empleadoExistente.idStatusEmpleado = registroDTO.idStatusEmpleado;


            // Si el password ha cambiado, recalculamos el hash y el salt
            if (!string.IsNullOrWhiteSpace(registroDTO.password))
            {
                using var hmac = new HMACSHA512();
                empleadoExistente.passwordHasH = hmac.ComputeHash(Encoding.UTF8.GetBytes(registroDTO.password));
                empleadoExistente.passwordSalt = hmac.Key;
            }

            return await _repository.Modificar(empleadoExistente);
        }

        public async Task<Empleado?> Eliminar(string numNomina)
        {
            var empleadoExistente = await _repository.Consultar(numNomina);

            if (empleadoExistente == null)
                return null; // Devuelve null si el empleado no existe

            return await _repository.Eliminar(numNomina);
        }

        public async Task<bool> EmpleadoExiste(string numNomina)
        {
            return await _repository.EmpleadoExiste(numNomina);
        }

        public ResultadoLogin EmpleadoExisteLogin(LoginDTO login)
        {
            bool esExitosoLogin = true;
            var empleadoTemp = _repository.EmpleadoExisteLogin(login.numNomina);

            using var hmac = new HMACSHA512(empleadoTemp.passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != empleadoTemp.passwordHasH[i]) esExitosoLogin = false;
            }

            var resultado = new ResultadoLogin
            {
                esLoginExitoso = esExitosoLogin,
                empleado = empleadoTemp,
            };

            return resultado;

        }

        Task<Empleado> IEmpleadoService.Consultar(string numNomina)
        {
            return _repository.Consultar(numNomina);
        }

        public Task<IEnumerable<Empleado>> ConsultarPorStatus(int idStatusEmpleado)
        {
            return _repository.ConsultarPorStatus(idStatusEmpleado);
        }

        public async Task<Empleado?> ConsultarPorId(int idEmpleado)
        {
            return await _repository.ConsultarPorId(idEmpleado);
        }

        //Servicio para cargar empleados desde un archivo Exce
    }
}
