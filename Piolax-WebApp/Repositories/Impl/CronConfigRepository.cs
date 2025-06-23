using Microsoft.EntityFrameworkCore;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories.Impl
{
    public class CronConfigRepository(AppDbContext dbContext) : ICronConfigRepository
    {
        private readonly AppDbContext _context = dbContext;

        public async Task<string> ObtenerHorarioPorNombre(string nombreCronConfig)
        {
            var cronConfig = await _context.CronConfig
                .Where(c => c.nombreCronConfig == nombreCronConfig)
                .Select(c => c.horaCron)
                .FirstOrDefaultAsync();
            if (cronConfig == null)
            {
                throw new KeyNotFoundException(
                    $"No se encontró la configuración '{nombreCronConfig}'.");
            }

            return cronConfig;
        }

        public async Task<IEnumerable<CronConfig>> GetAll()
        {
            return await _context.CronConfig.ToListAsync();
        }

        public async Task ActualizarHora(string nombreCronConfig, string horaCron)
        {
            var cronConfig = await _context.CronConfig
                .Where(c => c.nombreCronConfig == nombreCronConfig)
                .FirstOrDefaultAsync();

            if (cronConfig == null)
            {
                throw new KeyNotFoundException($"No se encontró la configuración de cron con el nombre: {nombreCronConfig}");
            }

            cronConfig.horaCron = horaCron;
            await _context.SaveChangesAsync();

        }
    }
}
