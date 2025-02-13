using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Piolax_WebApp.Hubs;
using Piolax_WebApp.Repositories; // Aquí se encuentra AppDbContext
using Piolax_WebApp.Models; // Aquí se encuentra la entidad Solicitud
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Piolax_WebApp.BackgroundServices
{
    public class NewRequestNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NewRequestNotificationService> _logger;

        public NewRequestNotificationService(IServiceScopeFactory scopeFactory,
                                             IHubContext<NotificationHub> hubContext,
                                             ILogger<NewRequestNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        // Buscar solicitudes nuevas (aquellas que no se han notificado)
                        var newRequests = await dbContext.Solicitudes
                            .Where(s => !s.notificado)
                            .ToListAsync(stoppingToken);

                        foreach (var solicitud in newRequests)
                        {
                            // Envía la notificación vía SignalR a todos los clientes
                            await _hubContext.Clients.All.SendAsync("ReceiveNewRequest", new
                            {
                                idSolicitud = solicitud.idSolicitud,
                                descripcion = solicitud.descripcion,
                                fechaSolicitud = solicitud.fechaSolicitud
                            }, stoppingToken);

                            // Marca la solicitud como notificada para evitar duplicados
                            solicitud.notificado = true;
                        }

                        if (newRequests.Any())
                        {
                            await dbContext.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en NewRequestNotificationService");
                }

                // Espera 30 segundos antes de volver a verificar
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
