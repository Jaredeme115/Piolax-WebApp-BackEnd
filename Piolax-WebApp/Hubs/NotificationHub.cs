using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Piolax_WebApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Piolax_WebApp.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        // Método general para enviar notificaciones
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        // Método específico para notificaciones de nuevas solicitudes
        public async Task SendRequestNotification(int idSolicitud, string descripcion)
        {
            await Clients.All.SendAsync("ReceiveRequestNotification", "Nueva solicitud asignada", idSolicitud, descripcion);
        }

        // Método para notificaciones de bajo stock
        public async Task SendLowStockNotification(int idRefaccion, string nombreProducto, int stockActual, int stockMinimo)
        {
            string mensaje = $"Bajo stock de {nombreProducto} - Actual: {stockActual} / Mínimo: {stockMinimo}";

            // Enviar solo a los gestores de inventario especificados
            await Clients.Group("GestoresInventario").SendAsync("LowStockAlert", new
            {
                idRefaccion,
                nombreProducto,
                cantidadActual = stockActual,
                cantidadMin = stockMinimo,
                mensaje
            });
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("🟡 Entrando a OnConnectedAsync");

            // 1) Agrupar a cada conexión por usuario (para notificaciones individuales)
            var idEmpleado = Context.UserIdentifier;
            Console.WriteLine($"[Hub] Conexión añadida a User_{idEmpleado}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{idEmpleado}");

            // 2) Agrupar por área + rol
            var idAreas = Context.User.FindAll("idArea").Select(c => c.Value).ToList();
            var roles = Context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var idRoles = Context.User.FindAll("idRol").Select(c => c.Value).ToList();

            foreach (var idArea in idAreas)
            {
                // a) Técnicos de Mantenimiento (idArea = 5)
                if (idArea == "5")
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Mantenimiento");
                    Console.WriteLine($"[Hub] Conexión añadida a Mantenimiento");
                }

                // b) Assistant Managers de esa área
                if (roles.Contains("Assistant Manager"))
                {
                    var grp = $"Area_{idArea}_Assistant";
                    await Groups.AddToGroupAsync(Context.ConnectionId, grp);
                    Console.WriteLine($"[Hub] Conexión añadida a {grp}");
                }

                // c) Supervisor de esa área
                if (roles.Contains("Supervisor"))
                {
                    var grp = $"Area_{idArea}_Supervisor";
                    await Groups.AddToGroupAsync(Context.ConnectionId, grp);
                    Console.WriteLine($"[Hub] Conexión añadida a {grp}");
                }

                // d) Grupos combinados por área y rol (para notificaciones específicas)
                foreach (var idRol in idRoles)
                {
                    var combinedGroup = $"Area_{idArea}_Rol_{idRol}";
                    await Groups.AddToGroupAsync(Context.ConnectionId, combinedGroup);
                    Console.WriteLine($"[Hub] Conexión añadida a {combinedGroup}");
                }
            }

            // Grupo especial para notificaciones de inventario
            bool esGestorInventario =
                (idAreas.Contains("5") && idRoles.Contains("7")) ||
                (idAreas.Contains("2") && idRoles.Contains("12"));

            if (esGestorInventario)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "GestoresInventario");
                Console.WriteLine($"[Hub] Conexión añadida a GestoresInventario");
            }

            await base.OnConnectedAsync();
        }

        public async Task JoinGroup(int idArea)
        {
            if (idArea == 5)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Mantenimiento");
                Console.WriteLine($"Cliente {Context.ConnectionId} unido al grupo Mantenimiento por idArea {idArea}");
            }
            else
            {
                Console.WriteLine($"Cliente {Context.ConnectionId} no pudo unirse al grupo debido a idArea {idArea}");
            }
        }

        // 1) Notificar nueva solicitud a los técnicos de Mantenimiento
        public async Task NewRequest(int idSolicitud, string descripcion, int idArea)
        {
            await Clients.Group("Mantenimiento")
                .SendAsync("ReceiveNewRequest", new { idSolicitud, descripcion });
        }

        // 2) Notificar al solicitante que su orden está lista para validación
        public async Task NotifyRequestReady(int idSolicitud, int idSolicitante)
        {
            await Clients.Group($"User_{idSolicitante}")
                .SendAsync("RequestReadyForApproval", new { idSolicitud });
        }

        // 3) Notificar a los roles superiores de área que hay una solicitud pendiente de validación
        public async Task NotifyAwaitingValidation(int idSolicitud, int idArea)
        {
            var dto = new { idSolicitud, idArea };
            await Clients.Group($"Area_{idArea}_Assistant")
                .SendAsync("RequestAwaitingValidation", dto);
            await Clients.Group($"Area_{idArea}_Supervisor")
                .SendAsync("RequestAwaitingValidation", dto);
        }

        // Unirse a un grupo para recibir actualizaciones de KPI por área
        public async Task JoinAreaKPIGroup(int idArea)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Area_{idArea}");
            Console.WriteLine($"Cliente {Context.ConnectionId} unido al grupo KPI para área {idArea}");
        }

        // Unirse a un grupo para recibir actualizaciones de KPI por máquina


        public async Task JoinMaquinaKPIGroup(int idMaquina)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Maquina_{idMaquina}");
            Console.WriteLine($"Cliente {Context.ConnectionId} unido al grupo KPI para máquina {idMaquina}");
        }

        // Salir de un grupo de KPI
        public async Task LeaveKPIGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"Cliente {Context.ConnectionId} salió del grupo KPI {groupName}");
        }



    }
}

