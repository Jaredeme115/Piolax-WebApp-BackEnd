using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class Usuario_Area_RolRepository(AppDbContext context) : IUsuario_Area_RolRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AsignarEmpleadoAreaRol(usuario_area_rol empleadoAreaRol)
        {
            _context.usuario_area_rol.Add(empleadoAreaRol);
            await _context.SaveChangesAsync();
        }

    }
}
