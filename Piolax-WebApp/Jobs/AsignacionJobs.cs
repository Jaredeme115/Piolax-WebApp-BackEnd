using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Jobs
{
    public class AsignacionJobs
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<AsignacionJobs> _log;
        private readonly TimeZoneInfo _tz = TimeZoneInfo.FindSystemTimeZoneById("America/Monterrey");

        public AsignacionJobs(AppDbContext ctx, ILogger<AsignacionJobs> log)
        {
            _ctx = ctx;
            _log = log;
        }

        public async Task PausarSolicitudesNoTomadas()
        {
            var ahora = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _tz);
            var pendientes = await _ctx.Solicitudes
              .Where(s => s.idStatusOrden == 3)
              .ToListAsync();

            foreach (var s in pendientes)
            {
                s.idStatusOrden = 7;
                s.fechaPausaSistema = ahora;
            }

            await _ctx.SaveChangesAsync();
        }

        public async Task ReanudarSolicitudes()
        {
            var ahora = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _tz);
            var pausadas = await _ctx.Solicitudes
              .Where(s => s.idStatusOrden == 7 && s.fechaPausaSistema != null)
              .ToListAsync();

            foreach (var s in pausadas)
            {
                // acumula minutos de pausa de sistema
                s.tiempoEsperaPausaSistema += (ahora - s.fechaPausaSistema.Value).TotalMinutes;
                s.idStatusOrden = 3;
                s.fechaPausaSistema = null;
            }

            await _ctx.SaveChangesAsync();
        }
    }
}
