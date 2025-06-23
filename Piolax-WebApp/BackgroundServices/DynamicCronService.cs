using Cronos;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.BackgroundServices
{
    public class DynamicCronService: BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _configName;
        private readonly Func<IServiceProvider, Task> _jobAction;

        public DynamicCronService(
            IServiceScopeFactory scopeFactory,
            string configName,
            Func<IServiceProvider, Task> jobAction
        )
        {
            _scopeFactory = scopeFactory;
            _configName = configName;
            _jobAction = jobAction;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Monterrey");
            while (!stoppingToken.IsCancellationRequested)
            {
                CronExpression cron;
                using (var scope = _scopeFactory.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<ICronConfigRepository>();
                    var expr = await repo.ObtenerHorarioPorNombre(_configName);
                    cron = CronExpression.Parse(expr, CronFormat.Standard);
                }

                var nowUtc = DateTimeOffset.UtcNow;
                var next = cron.GetNextOccurrence(nowUtc, tz);
                if (next == null) break;

                var delay = next.Value - nowUtc;
                await Task.Delay(delay, stoppingToken);

                using (var scope = _scopeFactory.CreateScope())
                    await _jobAction(scope.ServiceProvider);
            }
        }
    }
}
