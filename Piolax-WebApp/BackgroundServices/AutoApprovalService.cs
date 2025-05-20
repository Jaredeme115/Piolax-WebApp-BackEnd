using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Piolax_WebApp.Services.BackgroundServices
{
    public class AutoApprovalService : BackgroundService
    {
        private readonly ILogger<AutoApprovalService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly PeriodicTimer _timer;

        public AutoApprovalService(ILogger<AutoApprovalService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            // Crear un temporizador que se ejecuta cada minuto para verificar solicitudes pendientes
            _timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de aprobación automática iniciado a las {time}", DateTimeOffset.Now);

            try
            {
                while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
                {
                    await ProcessPendingApprovals();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Servicio de aprobación automática detenido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el servicio de aprobación automática");
            }
        }

        private async Task ProcessPendingApprovals()
        {
            _logger.LogInformation("Verificando solicitudes pendientes para aprobación automática a las {time}", DateTimeOffset.Now);

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var solicitudService = scope.ServiceProvider.GetRequiredService<ISolicitudService>();

            try
            {
                // Buscar las asignaciones con técnicos que tengan idStatusAprobacionTecnico = 1 
                // y cuya fecha de aprobación sea hace más de 15 minutos
                var now = DateTime.Now;
                var asignacionesParaAutoAprobar = await dbContext.Asignacion_Tecnico
                    .Where(at => at.idStatusAprobacionTecnico == 1 &&
                                 at.horaTermino != null &&
                                 at.horaTermino.AddMinutes(2) < now)
                    .Select(at => new
                    {
                        at.idAsignacion,
                        at.Asignacion.idSolicitud,
                        at.Asignacion.Solicitud.idStatusAprobacionSolicitante,
                        at.Asignacion.Solicitud.idStatusOrden
                    })
                    .Where(s => s.idStatusAprobacionSolicitante != 1 && s.idStatusOrden != 1) // Solo las que no estén ya aprobadas
                    .ToListAsync();

                foreach (var item in asignacionesParaAutoAprobar)
                {
                    _logger.LogInformation(
                        "Aprobando automáticamente la solicitud {idSolicitud} después de 2 minutos",
                        item.idSolicitud);

                    // Actualizar los estados a "Aprobado" (valor 1)
                    await solicitudService.ModificarEstatusAprobacionSolicitante(item.idSolicitud, 1);

                    // La llamada a ModificarEstatusAprobacionSolicitante ya actualiza idStatusOrden a 1 
                    // y todas las asignaciones relacionadas
                }

                if (asignacionesParaAutoAprobar.Any())
                {
                    _logger.LogInformation(
                        "Se aprobaron automáticamente {count} solicitudes",
                        asignacionesParaAutoAprobar.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar aprobaciones automáticas");
            }
        }
    }
}