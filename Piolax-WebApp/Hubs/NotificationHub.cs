using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Piolax_WebApp.Hubs
{
    public class NotificationHub: Hub
    {
        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();

            var idEmpleado = http?.Request.Query["idEmpleado"].ToString();
            var rol = http?.Request.Query["rol"].ToString();
            var idArea = http?.Request.Query["idArea"].ToString();

            if (!string.IsNullOrEmpty(idEmpleado))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Empleado-{idEmpleado}");

            if (!string.IsNullOrEmpty(rol))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Rol-{rol}");

            if (!string.IsNullOrEmpty(rol) && !string.IsNullOrEmpty(idArea))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Area-{idArea}-Rol-{rol}");

            await base.OnConnectedAsync();
        }
    }
}
