using System.Security.Cryptography;
using System.Text;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;

namespace Piolax_WebApp.Services.Impl
{
    public class EmpleadoService(IEmpleadoRepository repository) : IEmpleadoService
    {
        private readonly IEmpleadoRepository _repository = repository;

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
                passwordHasH = hmac.ComputeHash(Encoding.UTF8.GetBytes(registro.password)),
                passwordSalt = hmac.Key
            };

            return await _repository.Agregar(empleado);
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
    }
}
