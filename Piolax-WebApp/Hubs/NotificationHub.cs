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
    }
}
