using Microsoft.AspNetCore.SignalR;

namespace Piolax_WebApp.Hubs
{
    public class AsignacionHub: Hub
    {
        // Opcional: método para que un cliente se una a un grupo de una asignación
        public async Task JoinAsignacionGroup(int idAsignacion)
        {
            if (idAsignacion <= 0)
            {
                throw new ArgumentException("El idAsignacion debe ser mayor que cero.");
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, idAsignacion.ToString());
        }


        // Opcional: método para salir del grupo
        public async Task LeaveAsignacionGroup(int idAsignacion)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, idAsignacion.ToString());
        }
    }
}
