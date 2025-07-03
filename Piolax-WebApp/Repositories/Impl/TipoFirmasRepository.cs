using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class TipoFirmasRepository(AppDbContext context) : ITipoFirmasRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<TipoFirmas> RegistrarTipoFirma(TipoFirmas tipoFirmas)
        {
            await _context.TipoFirmas.AddAsync(tipoFirmas);
            await _context.SaveChangesAsync();
            return tipoFirmas;

        }

        public async Task<TipoFirmas> ModificarTipoFirma(int idTipoFirma)
        {
            var tipoFirma = await _context.TipoFirmas.FindAsync(idTipoFirma);
            if (tipoFirma == null)
            {
                return null;
            }
            // Aquí podrías modificar las propiedades de tipoFirma según sea necesario
            // Por ejemplo: tipoFirma.nombreTipoFirma = "Nuevo Nombre";
            tipoFirma.nombreTipoFirma = "Modificado"; // Ejemplo de modificación
            await _context.SaveChangesAsync();
            return tipoFirma;
        }

        public async Task<bool> EliminarTipoFirma(int idTipoFirma)
        {
            var tipoFirma = await _context.TipoFirmas.FindAsync(idTipoFirma);
            if (tipoFirma == null)
            {
                return false;
            }
            _context.TipoFirmas.Remove(tipoFirma);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
