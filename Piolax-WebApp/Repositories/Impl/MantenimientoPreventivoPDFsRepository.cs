using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class MantenimientoPreventivoPDFsRepository (AppDbContext context) : IMantenimientoPreventivoPDFsRepository
    {
        private AppDbContext _context = context;

        public async Task<MantenimientoPreventivoPDFs> AgregarMantenimientoPreventivoPDFs(MantenimientoPreventivoPDFs mantenimientoPreventivoPDFs)
        {
            _context.MantenimientoPreventivoPDFs.Add(mantenimientoPreventivoPDFs);
            await _context.SaveChangesAsync();
            return mantenimientoPreventivoPDFs;
        }

        public async  Task<IEnumerable<MantenimientoPreventivoPDFs>> ObtenerTodosLosMantenimientoPreventivoPDFs()
        {
            return await _context.MantenimientoPreventivoPDFs.ToListAsync();
        }

        public async Task<MantenimientoPreventivoPDFs?> ObtenerMantenimientoPreventivoPDFsPorID(int idMPPDF)
        {
            return await _context.MantenimientoPreventivoPDFs.FirstOrDefaultAsync(mppdf => mppdf.idMPPDF == idMPPDF);
        }

        public async Task<MantenimientoPreventivoPDFs> EliminarMantenimientoPreventivoPDFs(int idMPPDF)
        {
            var mantenimientoPreventivoPDFs = await _context.MantenimientoPreventivoPDFs
                .Where(mppdf => mppdf.idMPPDF == idMPPDF)
                .FirstOrDefaultAsync();

            if (mantenimientoPreventivoPDFs == null)
            {
                return null;
            }

            _context.MantenimientoPreventivoPDFs.Remove(mantenimientoPreventivoPDFs);

            try
            {
                await _context.SaveChangesAsync();
                return mantenimientoPreventivoPDFs;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al eliminar la refacción.", ex);

            }
        }

        public async Task<MantenimientoPreventivoPDFs> ConsultarPorNombrePDF(string nombrePDF)
        {
            return await _context.MantenimientoPreventivoPDFs.FirstOrDefaultAsync(mppdf => mppdf.nombrePDF == nombrePDF);
        }

        public async Task<IEnumerable<MantenimientoPreventivoPDFs>> ObtenerPDFsPorMantenimientoPreventivo(int idMP)
        {
            return await _context.MantenimientoPreventivoPDFs
                .Where(mppdf => mppdf.idMP == idMP)
                .ToListAsync();
        }


    }
}
