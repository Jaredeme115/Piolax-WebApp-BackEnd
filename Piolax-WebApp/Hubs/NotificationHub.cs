using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Piolax_WebApp.Hubs
{
    
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
        private static readonly string[] InventoryGroupNames = { "GestoresInventario" };

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        /*public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("User connected: {ConnectionId}", Context.ConnectionId);

            // Collect all groups to add
            var groups = new List<string>();

            // 1) Individual user group
            var userId = Context.UserIdentifier;
            groups.Add($"User_{userId}");

            // 2) Area+role groups
            var areas = Context.User.FindAll("idArea").Select(c => c.Value);
            var roles = Context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
            var roleIds = Context.User.FindAll("idRol").Select(c => c.Value);

            foreach (var area in areas)
            {
                // Maintenance technicians
                if (area == "5") groups.Add("Mantenimiento");

                // Area-level Assistant and Supervisor
                if (roles.Contains("Assistant Manager")) groups.Add($"Area_{area}_Assistant");
                if (roles.Contains("Supervisor")) groups.Add($"Area_{area}_Supervisor");

                // Combined area-role groups
                foreach (var roleId in roleIds)
                {
                    groups.Add($"Area_{area}_Rol_{roleId}");
                }
            }

            // 3) Inventory managers (special)
            if ((areas.Contains("5") && roleIds.Contains("7")) ||
                (areas.Contains("2") && roleIds.Contains("12")))
            {
                groups.AddRange(InventoryGroupNames);
            }

            // Add to all groups in parallel
            var tasks = groups.Select(g => Groups.AddToGroupAsync(Context.ConnectionId, g));
            await Task.WhenAll(tasks);

            await base.OnConnectedAsync();
        }*/

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("User connected: {ConnectionId}", Context.ConnectionId);

            try
            {
                // Collect all groups to add
                var groups = new List<string>();

                // 1) Individual user group
                var userId = Context.UserIdentifier;
                if (!string.IsNullOrEmpty(userId))
                {
                    groups.Add($"User_{userId}");

                    // Solo procesar claims si hay usuario autenticado
                    if (Context.User?.Identity?.IsAuthenticated == true)
                    {
                        // 2) Area+role groups
                        var areas = Context.User.FindAll("idArea").Select(c => c.Value).ToList();
                        var roles = Context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                        var roleIds = Context.User.FindAll("idRol").Select(c => c.Value).ToList();

                        foreach (var area in areas)
                        {
                            // Maintenance technicians
                            if (area == "5") groups.Add("Mantenimiento");

                            // Area-level Assistant and Supervisor
                            if (roles.Contains("Assistant Manager")) groups.Add($"Area_{area}_Assistant");
                            if (roles.Contains("Supervisor")) groups.Add($"Area_{area}_Supervisor");

                            // Combined area-role groups
                            foreach (var roleId in roleIds)
                            {
                                groups.Add($"Area_{area}_Rol_{roleId}");
                            }
                        }

                        // 3) Inventory managers (special)
                        if ((areas.Contains("5") && roleIds.Contains("7")) ||
                            (areas.Contains("2") && roleIds.Contains("12")))
                        {
                            groups.AddRange(InventoryGroupNames);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Usuario conectado pero no autenticado: {ConnectionId}", Context.ConnectionId);
                    }
                }

                // Add to all groups in parallel if any
                if (groups.Any())
                {
                    var tasks = groups.Select(g => Groups.AddToGroupAsync(Context.ConnectionId, g));
                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en OnConnectedAsync para {ConnectionId}: {Message}", Context.ConnectionId, ex.Message);
                // No re-lanzamos la excepción para permitir que la conexión continúe
            }

            await base.OnConnectedAsync();
        }

        // Generic join/leave group API
        public Task JoinGroupByName(string groupName)
        {
            _logger.LogInformation("Joining group {Group} for connection {ConnectionId}", groupName, Context.ConnectionId);
            return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public Task LeaveGroupByName(string groupName)
        {
            _logger.LogInformation("Leaving group {Group} for connection {ConnectionId}", groupName, Context.ConnectionId);
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        // Notification methods simplified to send to named group or all
        public Task SendNotification(string message)
            => Clients.All.SendAsync("ReceiveNotification", message);

        public Task SendLowStockNotification(int idRefaccion, string nombreProducto, int stockActual, int stockMinimo)
        {
            var mensaje = $"Bajo stock de {nombreProducto} - Actual: {stockActual} / Mínimo: {stockMinimo}";
            return Clients.Group(InventoryGroupNames.First())
                          .SendAsync("LowStockAlert", new
                          {
                              idRefaccion,
                              nombreProducto,
                              cantidadActual = stockActual,
                              cantidadMin = stockMinimo,
                              mensaje
                          });
        }

        public Task NewRequest(int idSolicitud, string descripcion)
            => Clients.Group("Mantenimiento").SendAsync("ReceiveNewRequest", new { idSolicitud, descripcion });

        public Task NotifyRequestReady(int idSolicitud, int idSolicitante)
            => Clients.Group($"User_{idSolicitante}")
                      .SendAsync("RequestReadyForApproval", new { idSolicitud });

        public Task NotifyAwaitingValidation(int idEmpleadoSolicitante, int idAreaSolicitante, int idSolicitud)
        {
            var dto = new { idSolicitud, estado = 4 };

            var tasks = new List<Task>
            {
                // Al creador
                Clients.Group($"User_{idEmpleadoSolicitante}")
                        .SendAsync("RequestAwaitingValidation", dto)
            };

            // A los roles 6,7,8 de la misma área
            foreach (var rol in new[] { 6, 7, 8 })
            {
                tasks.Add(
                  Clients.Group($"Area_{idAreaSolicitante}_Rol_{rol}")
                         .SendAsync("RequestAwaitingValidation", dto)
                );
            }

            return Task.WhenAll(tasks);
        }

        public Task VerifyConnection()
        {
            var info = new
            {
                connectionId = Context.ConnectionId,
                userId = Context.UserIdentifier,
                isAuthenticated = Context.User?.Identity?.IsAuthenticated ?? false,
                claims = Context.User?.Claims?.Select(c => new { c.Type, c.Value }).ToList()
            };

            return Clients.Caller.SendAsync("ConnectionStatus", info);
        }

        public Task Ping()
        {
            return Clients.Caller.SendAsync("Pong", DateTime.Now);
        }

    }
}
