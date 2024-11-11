using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class RolesRepository(AppDbContext context) : IRolesRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Roles> Consultar(int idRol)
        {
            return await _context.Roles.Where(p => p.idRol == idRol).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Roles>> ConsultarTodos()
        {
            var roles = await _context.Roles.ToListAsync();
            return roles;
        }

        public async Task<Roles> Registro(Roles rol)
        {
            await _context.Roles.AddAsync(rol);
            await _context.SaveChangesAsync();
            return rol;
        }

        public async Task<Roles> Modificar(int idRol, Roles rol)
        {
            _context.Update(rol);
            await _context.SaveChangesAsync();
            return rol;
        }

        public async Task<Roles> Eliminar(int idRol)
        {
            var rol = await _context.Roles.Where(p => p.idRol == idRol).FirstOrDefaultAsync();
            if (rol != null)
            {
                _context.Remove(rol);
                await _context.SaveChangesAsync();
            }
            return rol;
        }

        public Task<bool> RolExiste(int idRol)
        {
            return _context.Roles.AnyAsync(p => p.idRol == idRol);
        }

        public Task<bool> RolExisteRegistro(string nombreRol)
        {
            return _context.Roles.AnyAsync(p => p.nombreRol == nombreRol);
        }
    }
}
