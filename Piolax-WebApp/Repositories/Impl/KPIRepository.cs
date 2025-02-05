using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class KPIRepository(AppDbContext context): IKPIRepository
    {
        private readonly AppDbContext _context = context;

        public async Task GuardarKPIMantenimiento(KpisMantenimiento kpiMantenimiento)
        {
            _context.KpisMantenimiento.Add(kpiMantenimiento);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarKPIDetalles(int idKPIMantenimiento, List<KpisDetalle> kpiDetalles)
        {
            foreach (var detalle in kpiDetalles)
            {
                detalle.idKPIMantenimiento = idKPIMantenimiento;
                _context.KpisDetalles.Add(detalle);
            }
            await _context.SaveChangesAsync();
        }
    }
}
