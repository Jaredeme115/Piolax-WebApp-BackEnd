using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class KPIRepository(AppDbContext context): IKPIRepository
    {
        private readonly AppDbContext _context = context;

        // KPIs para Mantenimiento Correctivo
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
                _context.KpisDetalle.Add(detalle);
            }
            await _context.SaveChangesAsync();
        }

        // KPIs para Mantenimiento Preventivo

        public async Task GuardarKPIPreventivo(KpisMP kpisMP)
        {
            _context.KpisMP.Add(kpisMP);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarKPIDetallesMP(int idKPIMP, List<KpisMPDetalle> kpiDetalles)
        {
            foreach (var detalle in kpiDetalles)
            {
                detalle.idKPIMP = idKPIMP;
                _context.KpisMPDetalle.Add(detalle);
            }
            await _context.SaveChangesAsync();
        }
    }
}
