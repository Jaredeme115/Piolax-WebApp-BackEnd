using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class TurnosService(ITurnosRepository repository) : ITurnosService
    {
        private readonly ITurnosRepository _repository = repository;

        public Task<IEnumerable<Turnos>> ConsultarTodos()
        {
            return _repository.ConsultarTodos();
        }

        public Task<Turnos> Consultar(int idTurno)
        {
            return _repository.Consultar(idTurno);
        }

        public async Task<Turnos> Registro(TurnoDTO turno)
        {
            var turnos = new Turnos
            {
                descripcion = turno.descripcion,
                horaInicio = turno.horaInicio,
                horaFinal = turno.horaFinal
            };

            return await _repository.Registro(turnos);
        }

        public async Task<Turnos> Modificar(int idTurno, TurnoDTO turno)
        {
            var turnoExistente = await _repository.Consultar(idTurno);

            if (turnoExistente == null)
                return null; // Devuelve null si el turno no existe

            // Actualizamos los datos del turno
            turnoExistente.descripcion = turno.descripcion;
            turnoExistente.horaInicio = turno.horaInicio;
            turnoExistente.horaFinal = turno.horaFinal;

            return await _repository.Modificar(idTurno, turnoExistente);
        }

        public async Task<Turnos> Eliminar(int idTurno)
        {
            return await _repository.Eliminar(idTurno);
        }

        public async Task<bool> TurnoExiste(int idTurno)
        {
            return await _repository.TurnoExiste(idTurno);
        }

        public async Task<bool> TurnoExisteRegistro(string nombreTurno)
        {
            return await _repository.TurnoExisteRegistro(nombreTurno);
        }


    }
}
