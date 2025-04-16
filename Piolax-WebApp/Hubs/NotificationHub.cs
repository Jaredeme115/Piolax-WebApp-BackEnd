using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Piolax_WebApp.Hubs
{
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

        public override async Task OnConnectedAsync()
        {
            // Obtener todos los claims de idArea que tenga el usuario
            var idAreaClaims = Context.User?.FindAll("idArea");

            // Si alguno de los claims tiene el valor "5", se agrega al grupo "Mantenimiento"
            if (idAreaClaims != null && idAreaClaims.Any(claim => claim.Value == "5"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Mantenimiento");
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
                // Otros casos
            }
        }


    }
}

