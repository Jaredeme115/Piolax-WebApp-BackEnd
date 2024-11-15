using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface ITurnosRepository
    {
        Task<Turnos> Consultar(int idTurno);
        Task<IEnumerable<Turnos>> ConsultarTodos();
        Task<Turnos> Registro(Turnos turno);
        Task<Turnos> Modificar(int idTurno, Turnos turno);
        Task<Turnos> Eliminar(int idTurno);
        Task<bool> TurnoExiste(int idTurno);
        Task<bool> TurnoExisteRegistro(string descripcion);
    }
}
