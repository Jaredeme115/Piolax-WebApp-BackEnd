using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Piolax_WebApp.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task EnviarNotificacion(string tipo, string mensaje, string? rol = null)
        {
            if (!string.IsNullOrEmpty(rol))
            {
                await Clients.Group(rol).SendAsync("RecibirNotificacion", new { tipo, mensaje });
            }
            else
            {
                await Clients.All.SendAsync("RecibirNotificacion", new { tipo, mensaje });
            }
        }
    }
}


