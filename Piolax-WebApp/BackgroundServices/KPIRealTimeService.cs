using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Piolax_WebApp.Hubs;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Piolax_WebApp.BackgroundServices
{
    public class KPIRealTimeService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<KPIRealTimeService> _logger;
        private readonly TimeSpan _intervaloActualizacion = TimeSpan.FromMinutes(5); // Actualizar cada 5 minutos

        public KPIRealTimeService(
            IServiceScopeFactory scopeFactory,
            IHubContext<NotificationHub> hubContext,
            ILogger<KPIRealTimeService> logger)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de actualización de KPIs en tiempo real iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    // Obtener los servicios necesarios
                    var kpiDashboardService = scope.ServiceProvider.GetRequiredService<IKPIDashboardService>();
                    var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // 1. Obtener todas las áreas
                    var areas = await appDbContext.Areas.ToListAsync(stoppingToken);

                    // 2. Obtener todas las máquinas
                    var maquinas = await appDbContext.Maquinas.ToListAsync(stoppingToken);

                    _logger.LogInformation($"Actualizando KPIs para {areas.Count} áreas y {maquinas.Count} máquinas");

                    // 3. Para cada área, calcular los KPIs y enviarlos por SignalR
                    foreach (var area in areas)
                    {
                        try
                        {
                            // Obtener KPIs para esta área
                            var mtta = await kpiDashboardService.ObtenerMTTA(area.idArea);
                            var mttr = await kpiDashboardService.ObtenerMTTR(area.idArea);
                            var mtbf = await kpiDashboardService.ObtenerMTBF(area.idArea);
                            var totalDowntime = await kpiDashboardService.ObtenerTotalDowntime(area.idArea);

                            // Crear el paquete de datos
                            var kpiData = new
                            {
                                Timestamp = DateTime.Now,
                                Area = new { Id = area.idArea, Nombre = area.nombreArea },
                                KPIs = new[]
                                {
                                    mtta,
                                    mttr,
                                    mtbf,
                                    totalDowntime
                                }
                            };

                            // Enviar a todos los interesados en esta área
                            await _hubContext.Clients.Group($"Area_{area.idArea}")
                                .SendAsync("AreaKPIUpdate", kpiData, stoppingToken);

                            // También enviar al grupo general de mantenimiento
                            if (area.idArea == 5) // Asumimos que 5 es el ID de Mantenimiento
                            {
                                await _hubContext.Clients.Group("Mantenimiento")
                                    .SendAsync("AreaKPIUpdate", kpiData, stoppingToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error al actualizar KPIs para área {area.idArea}");
                        }
                    }

                    // 4. Para cada máquina, calcular los KPIs específicos
                    foreach (var maquina in maquinas)
                    {
                        try
                        {
                            // Obtener KPIs para esta máquina
                            var mtta = await kpiDashboardService.ObtenerMTTA(maquina.idArea, maquina.idMaquina);
                            var mttr = await kpiDashboardService.ObtenerMTTR(maquina.idArea, maquina.idMaquina);
                            var totalDowntime = await kpiDashboardService.ObtenerTotalDowntime(maquina.idArea, maquina.idMaquina);

                            // Crear el paquete de datos
                            var kpiData = new
                            {
                                Timestamp = DateTime.Now,
                                Maquina = new { Id = maquina.idMaquina, Nombre = maquina.nombreMaquina, Area = maquina.idArea },
                                KPIs = new[]
                                {
                                    mtta,
                                    mttr,
                                    totalDowntime
                                }
                            };

                            // Enviar a todos los interesados en esta máquina
                            await _hubContext.Clients.Group($"Maquina_{maquina.idMaquina}")
                                .SendAsync("MaquinaKPIUpdate", kpiData, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error al actualizar KPIs para máquina {maquina.idMaquina}");
                        }
                    }

                    _logger.LogInformation("Actualización de KPIs completada correctamente");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error general en el servicio de actualización de KPIs");
                }

                // Esperar hasta el próximo ciclo de actualización
                await Task.Delay(_intervaloActualizacion, stoppingToken);
            }
        }
    }
}
