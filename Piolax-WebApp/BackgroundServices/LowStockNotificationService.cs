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
    public class LowStockNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<LowStockNotificationService> _logger;
        private readonly TimeSpan _intervaloChequeo = TimeSpan.FromMinutes(1); // Verificar cada 1 minuto(s)
        private readonly TimeSpan _intervaloReenvio = TimeSpan.FromHours(24); // Reenviar cada 24 horas

        public LowStockNotificationService(IServiceScopeFactory scopeFactory,
                                           IHubContext<NotificationHub> hubContext,
                                           ILogger<LowStockNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de notificaciones de bajo stock iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // 1) Refacciones que llegaron recientemente a bajo stock (nunca notificadas)
                    var nuevasBajas = await db.Inventario
                        .Where(i => i.cantidadActual <= i.cantidadMin && !i.bajoStockNotificado)
                        .ToListAsync(stoppingToken);

                    // 2) Refacciones que siguen en bajo stock y necesitan renotificación
                    var pendientesReenvio = await db.Inventario
                        .Where(i => i.cantidadActual <= i.cantidadMin &&
                               i.bajoStockNotificado &&
                               (i.ultimaNotificacionBajoStock == null ||
                                DateTime.UtcNow - i.ultimaNotificacionBajoStock >= _intervaloReenvio))
                        .ToListAsync(stoppingToken);

                    // Procesar notificaciones nuevas
                    foreach (var refaccion in nuevasBajas)
                    {
                        await EnviarNotificacionBajoStock(refaccion, "¡Alerta! Nivel crítico de inventario", stoppingToken);

                        // Marcar como notificada
                        refaccion.bajoStockNotificado = true;
                        refaccion.ultimaNotificacionBajoStock = DateTime.UtcNow;

                        _logger.LogInformation($"Primera notificación por bajo stock: {refaccion.nombreProducto} (ID: {refaccion.idRefaccion})");
                    }

                    // Procesar renotificaciones
                    foreach (var refaccion in pendientesReenvio)
                    {
                        await EnviarNotificacionBajoStock(refaccion, "Recordatorio: Continúa en nivel crítico de inventario", stoppingToken);

                        // Actualizar fecha de última notificación
                        refaccion.ultimaNotificacionBajoStock = DateTime.UtcNow;

                        _logger.LogInformation($"Renotificación por bajo stock: {refaccion.nombreProducto} (ID: {refaccion.idRefaccion})");
                    }

                    // Guardar cambios si hubo notificaciones
                    if (nuevasBajas.Any() || pendientesReenvio.Any())
                    {
                        await db.SaveChangesAsync(stoppingToken);
                    }

                    // Esperar antes de la siguiente verificación
                    await Task.Delay(_intervaloChequeo, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el servicio de notificaciones de bajo stock");

                    // Esperar un poco antes de reintentar en caso de error
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        /*private async Task EnviarNotificacionBajoStock(Inventario refaccion, string tipoAlerta, CancellationToken stoppingToken)
        {
            // Enviar notificación detallada al grupo de Mantenimiento
            await _hubContext.Clients.Group("Mantenimiento")
                .SendAsync("LowStockAlert", new
                {
                    mensaje = tipoAlerta,
                    refaccion.idRefaccion,
                    refaccion.nombreProducto,
                    refaccion.cantidadActual,
                    refaccion.cantidadMin,
                    refaccion.proveedor,
                    refaccion.numParte,
                    diasSinReposicion = refaccion.ultimaNotificacionBajoStock.HasValue ?
                        (int)(DateTime.UtcNow - refaccion.ultimaNotificacionBajoStock.Value).TotalDays : 0
                }, stoppingToken);

            // Enviar notificación general también para que aparezca en el panel de notificaciones
            string mensajeGeneral = $"{tipoAlerta}: {refaccion.nombreProducto} - Stock: {refaccion.cantidadActual}/{refaccion.cantidadMin}";
            await _hubContext.Clients.Group("Mantenimiento")
                .SendAsync("ReceiveNotification", mensajeGeneral, stoppingToken);
        }*/

        private async Task EnviarNotificacionBajoStock(Inventario refaccion, string tipoAlerta, CancellationToken stoppingToken)
        {
            // Enviar notificación detallada solo al grupo de gestores de inventario
            await _hubContext.Clients.Group("GestoresInventario")
                .SendAsync("LowStockAlert", new
                {
                    mensaje = tipoAlerta,
                    refaccion.idRefaccion,
                    refaccion.nombreProducto,
                    refaccion.cantidadActual,
                    refaccion.cantidadMin,
                    refaccion.proveedor,
                    refaccion.numParte,
                    diasSinReposicion = refaccion.ultimaNotificacionBajoStock.HasValue ?
                        (int)(DateTime.UtcNow - refaccion.ultimaNotificacionBajoStock.Value).TotalDays : 0
                }, stoppingToken);

            // Enviar notificación general también solo a los gestores
            string mensajeGeneral = $"{tipoAlerta}: {refaccion.nombreProducto} - Stock: {refaccion.cantidadActual}/{refaccion.cantidadMin}";
            await _hubContext.Clients.Group("GestoresInventario")
                .SendAsync("ReceiveNotification", mensajeGeneral, stoppingToken);
        }
    }
}

