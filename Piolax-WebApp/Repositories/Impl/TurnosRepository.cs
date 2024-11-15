using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class TurnosRepository(AppDbContext context) : ITurnosRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Turnos> Consultar(int idTurno)
        {
            return await _context.Turnos.Where(p => p.idTurno == idTurno).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Turnos>> ConsultarTodos()
        {
            var turnos = await _context.Turnos.ToListAsync();
            return turnos;
        }

        public async Task<Turnos> Registro(Turnos turno)
        {
            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();
            return turno;
        }

        public async Task<Turnos> Modificar(int idTurno, Turnos turno)
        {
            _context.Entry(turno).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return turno;
        }

        public async Task<Turnos> Eliminar(int idTurno)
        {
            var turno = await _context.Turnos.Where(p => p.idTurno == idTurno).FirstOrDefaultAsync();
            _context.Turnos.Remove(turno);
            await _context.SaveChangesAsync();
            return turno;
        }

        public async Task<bool> TurnoExiste(int idTurno)
        {
            return await _context.Turnos.AnyAsync(p => p.idTurno == idTurno);
        }

        public async Task<bool> TurnoExisteRegistro(string descripcion)
        {
            return await _context.Turnos.AnyAsync(p => p.descripcion == descripcion);
        }
    }
}
