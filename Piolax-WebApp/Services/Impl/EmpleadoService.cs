﻿using System.Security.Cryptography;
using System.Text;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Piolax_WebApp.Services.Impl
{
    public class EmpleadoService(IEmpleadoRepository repository, IUsuario_Area_RolRepository usuario_Area_RolRepository) : IEmpleadoService
    {
        private readonly IEmpleadoRepository _repository = repository;
        private readonly IUsuario_Area_RolRepository _usuario_Area_RolRepository = usuario_Area_RolRepository;

        public Task<IEnumerable<Empleado>> ConsultarTodos()
        {
            return _repository.ConsultarTodos();
        }

        public async Task<Empleado> Registro(RegistroDTO registro)
        {
            using var hmac = new HMACSHA512();
            var empleado = new Empleado
            {
                numNomina = registro.numNomina,
                nombre = registro.nombre,
                apellidoPaterno = registro.apellidoPaterno,
                apellidoMaterno = registro.apellidoMaterno,
                email = registro.email,
                telefono = registro.telefono,
                fechaIngreso = registro.fechaIngreso,
                idStatusEmpleado = registro.idStatusEmpleado,
                passwordHasH = hmac.ComputeHash(Encoding.UTF8.GetBytes(registro.password)),
                passwordSalt = hmac.Key,
               
            };

            var empleadoRegistrado = await _repository.Agregar(empleado);

            // Agregar la relación de área y rol
            if (registro.idArea != null && registro.idRol != null)
            {
                var empleadoAreaRol = new usuario_area_rol
                {
                    idEmpleado = empleadoRegistrado.idEmpleado,
                    idArea = registro.idArea,
                    idRol = registro.idRol
                };

                await usuario_Area_RolRepository.AsignarEmpleadoAreaRol(empleadoAreaRol);
            }

            return empleadoRegistrado;
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
            empleadoExistente.idStatusEmpleado = registroDTO.idStatusEmpleado;


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

    }
}
