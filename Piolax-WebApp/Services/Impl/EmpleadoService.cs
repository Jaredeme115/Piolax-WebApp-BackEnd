using System.Security.Cryptography;
using System.Text;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using IronXL;
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
            empleadoExistente.idStatusEmpleado = registroDTO.idStatusEmpleado;

            //🔑 Validación de contraseña
            if (!string.IsNullOrWhiteSpace(registroDTO.passwordNuevo))
            {
                // Verifica que se haya proporcionado la contraseña actual
                if (string.IsNullOrWhiteSpace(registroDTO.password))
                    throw new InvalidOperationException("Debes proporcionar tu contraseña actual para cambiarla.");

                // Valida contraseña actual
                using var hmacActual = new HMACSHA512(empleadoExistente.passwordSalt);
                var hashActual = hmacActual.ComputeHash(Encoding.UTF8.GetBytes(registroDTO.password));

                if (!hashActual.SequenceEqual(empleadoExistente.passwordHasH))
                    throw new InvalidOperationException("La contraseña actual es incorrecta.");

                // Evita que la nueva contraseña sea igual a la anterior
                var nuevoHashTemporal = hmacActual.ComputeHash(Encoding.UTF8.GetBytes(registroDTO.passwordNuevo));
                if (nuevoHashTemporal.SequenceEqual(empleadoExistente.passwordHasH))
                    throw new InvalidOperationException("La nueva contraseña no puede ser igual a la contraseña actual.");

                // Si pasa las validaciones, actualiza claramente la contraseña nueva
                using var nuevoHmac = new HMACSHA512();
                empleadoExistente.passwordSalt = nuevoHmac.Key;
                empleadoExistente.passwordHasH = nuevoHmac.ComputeHash(Encoding.UTF8.GetBytes(registroDTO.passwordNuevo));
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

        //Servicio para cargar empleados desde un archivo Excel
        public async Task<string> RegistrarEmpleadosDesdeExcel(IFormFile filePath)
        {
            if (filePath == null || filePath.Length == 0)
                throw new ArgumentException("El archivo es inválido.");

            var errores = new List<string>(); // Para registrar errores de procesamiento
            int empleadosCargados = 0;

            try
            {
                var workbook = WorkBook.Load(filePath.OpenReadStream()); // Carga el archivo Excel
                var worksheet = workbook.WorkSheets.First(); // Obtiene la primera hoja del archivo

                for (int row = 2; row <= worksheet.RowCount; row++) // Itera desde la fila 2
                {
                    try
                    {
                        var registroDTO = new RegistroDTO
                        {
                            numNomina = worksheet[$"A{row}"].StringValue,
                            nombre = worksheet[$"B{row}"].StringValue,
                            apellidoPaterno = worksheet[$"C{row}"].StringValue,
                            apellidoMaterno = worksheet[$"D{row}"].StringValue,
                            telefono = worksheet[$"E{row}"].StringValue,
                            email = worksheet[$"F{row}"].StringValue,
                            fechaIngreso = DateOnly.FromDateTime(DateTime.Parse(worksheet[$"G{row}"].StringValue)),
                            password = worksheet[$"H{row}"].StringValue, 
                            idStatusEmpleado = int.Parse(worksheet[$"I{row}"].StringValue),
                            idArea = int.Parse(worksheet[$"J{row}"].StringValue),
                            idRol = int.Parse(worksheet[$"K{row}"].StringValue)
                        };

                        // Reutiliza el método Registro para insertar el empleado
                        await Registro(registroDTO);
                        empleadosCargados++;
                    }
                    catch (Exception ex)
                    {
                        // Agrega detalles del error para la fila actual
                        errores.Add($"Error en fila {row}: {ex.Message}");
                    }
                }

                var resultado = $"{empleadosCargados} empleados cargados correctamente.";
                if (errores.Any())
                {
                    resultado += $" Se encontraron errores en {errores.Count} filas: {string.Join("; ", errores)}";
                }
                return resultado;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al procesar el archivo Excel.", ex);
            }
        }
    }
}
