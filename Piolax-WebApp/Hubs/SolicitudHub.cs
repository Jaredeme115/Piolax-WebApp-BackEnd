using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Piolax_WebApp.Hubs
{
    public class SolicitudHub: Hub
    {
        public async Task NotificarActualizacionSolicitudes(int idArea, List<int> idRoles)
        {
            await Clients.All.SendAsync("RecibirActualizacionSolicitudes", idArea, idRoles);
        }

        // Método para unirse a un grupo específico de área
        public async Task UnirseGrupoArea(int idArea)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Area_{idArea}");
        }

        // Método para salir de un grupo de área
        public async Task SalirGrupoArea(int idArea)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Area_{idArea}");
        }
    }
}
