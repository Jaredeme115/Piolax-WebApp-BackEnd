using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Piolax_WebApp.Hubs
{
    public class NotificationHub: Hub
    {
        // Este método lo pueden llamar los clientes si se requiere (opcional)
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendNotificationToGroup(string groupName, string titulo, string mensaje)
        {
            await Clients.Group(groupName).SendAsync("RecibirNotificacion", new
            {
                titulo,
                mensaje,
                fecha = DateTime.Now
            });
        }
    }
}
