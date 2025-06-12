using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Jobs
{
    public class AsignacionJobs
    {
        private readonly AppDbContext _ctx;
        public AsignacionJobs(AppDbContext ctx) => _ctx = ctx;

        public async Task PausarFinDeSemana()
        {
            var toPause = await _ctx.Asignaciones
               .Where(a => a.idStatusAsignacion == 4)
               .ToListAsync();
            foreach (var a in toPause)
            {
                a.ultimaVezSinTecnico = DateTime.Now;
                a.idStatusAsignacion = 6; // Pausada por Sistema
            }

            await _ctx.SaveChangesAsync();
        }

        public async Task ReanudarLunes()
        {
            var toResume = await _ctx.Asignaciones
               .Where(a => a.idStatusAsignacion == 6 && a.ultimaVezSinTecnico != null)
               .ToListAsync();
            foreach (var a in toResume)
            {
                a.tiempoEsperaAcumuladoMinutos +=
                  (DateTime.Now - a.ultimaVezSinTecnico.Value).TotalMinutes;
                a.ultimaVezSinTecnico = null;
                a.idStatusAsignacion = 4; // No tomada
            }
            await _ctx.SaveChangesAsync();
        }

    }
}
