using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Piolax_WebApp.Hubs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Piolax_WebApp.BackgroundServices
{
    public class PendingOrderMonitorService: BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IHubContext<NotificationHub> _hub;

        public PendingOrderMonitorService(IServiceProvider services, IHubContext<NotificationHub> hubContext)
        {
            _services = services;
            _hub = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Se ejecuta en loop hasta cancelar la aplicación
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<ISolicitudesRepository>();

                var ahora = DateTime.Now;
                // Traemos todas las que siguen en 3
                var pendientes = await repo.ObtenerSolicitudesEnStatus(3);

                foreach (var s in pendientes)
                {
                    var minutos = (ahora - s.fechaSolicitud).TotalMinutes;

                    if (minutos >= 15 && minutos < 30)
                    {
                        // Coordinador de Mantenimiento: idArea=5, idRol=19 → grupo "Area_5_Rol_19"
                        await _hub.Clients.Group("Area_5_Rol_19")
                                  .SendAsync("RecordatorioOrdenPendiente", new { s.idSolicitud, texto = "Orden sin tomar tras 15 minutos." });
                    }
                    else if (minutos >= 30 && minutos < 45)
                    {
                        // Assistant Manager Mantenimiento: idArea=5, idRol=7 → "Area_5_Rol_7"
                        await _hub.Clients.Group("Area_5_Rol_7")
                                  .SendAsync("RecordatorioOrdenPendiente", new { s.idSolicitud, texto = "Orden sin tomar tras 30 minutos." });
                    }
                    else if (minutos >= 45)
                    {
                        // Jefe general: idArea=15, idRol=15 → "Area_15_Rol_15"
                        await _hub.Clients.Group("Area_15_Rol_15")
                                  .SendAsync("RecordatorioOrdenPendiente", new { s.idSolicitud, texto = "Orden sin tomar tras 45 minutos." });
                    }
                }

                // Espera 5 minutos antes de la siguiente revisión
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}