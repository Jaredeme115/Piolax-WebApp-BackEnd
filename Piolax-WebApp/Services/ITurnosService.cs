using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface ITurnosService
    {
        Task<Turnos> Consultar(int idTurno);
        Task<IEnumerable<Turnos>> ConsultarTodos();
        Task<Turnos> Registro(TurnoDTO turno);
        Task<Turnos> Modificar(int idTurno, TurnoDTO turno);
        Task<Turnos> Eliminar(int idTurno);
        Task<bool> TurnoExiste(int idTurno);
        Task<bool> TurnoExisteRegistro(string nombreTurno);
    }
}
