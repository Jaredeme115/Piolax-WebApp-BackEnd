using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class ObservacionesMPRepository(AppDbContext context) : IObservacionesMPRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<ObservacionesMP> AgregarObservacion(ObservacionesMP observacion)
        {
            _context.ObservacionesMP.Add(observacion);
            await _context.SaveChangesAsync();
            return observacion;
        }

        public async Task<IEnumerable<ObservacionesMP>> ObtenerObservacionesPorMP(int idMP)
        {
            return await _context.ObservacionesMP
                .Where(obs => obs.idMP == idMP)
                .ToListAsync();
        }

        public async Task<ObservacionesMP?> EliminarObservacion(int idObservacionMP)
        {
            var observacion = await _context.ObservacionesMP
                .FirstOrDefaultAsync(obs => obs.idObservacionMP == idObservacionMP);

            if (observacion == null)
            {
                return null;
            }

            _context.ObservacionesMP.Remove(observacion);
            await _context.SaveChangesAsync();
            return observacion;
        }
    }
}
