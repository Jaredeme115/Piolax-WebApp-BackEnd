using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class MaquinasRepository(AppDbContext context) : IMaquinasRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Maquinas> Consultar(int idMaquina)
        {
            return await _context.Maquinas.Where(p => p.idMaquina == idMaquina).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Maquinas>> ConsultarTodos()
        {
            var maquinas = await _context.Maquinas.ToListAsync();
            return maquinas;
        }

        public async Task<Maquinas> Registro(Maquinas maquina)
        {
            await _context.Maquinas.AddAsync(maquina);
            await _context.SaveChangesAsync();
            return maquina;
        }

        public async Task<Maquinas> Modificar(int idMaquina, Maquinas maquina)
        {
            _context.Update(maquina);
            await _context.SaveChangesAsync();
            return maquina;
        }

        public async Task<Maquinas> Eliminar(int idMaquina)
        {
            var maquina = await _context.Maquinas.Where(p => p.idMaquina == idMaquina).FirstOrDefaultAsync();
            if (maquina != null)
            {
                _context.Remove(maquina);
                await _context.SaveChangesAsync();
            }
            return maquina;
        }

        public Task<bool> MaquinaExiste(int idMaquina)
        {
            return _context.Maquinas.AnyAsync(p => p.idMaquina == idMaquina);
        }

        public Task<bool> MaquinaExisteRegistro(string nombreMaquina)
        {
            return _context.Maquinas.AnyAsync(p => p.nombreMaquina == nombreMaquina);
        }

        public async Task<IEnumerable<Maquinas>> ConsultarPorArea(int idArea)
        {
            return await _context.Maquinas.Where(p => p.idArea == idArea).ToListAsync();
        }
    }
}
