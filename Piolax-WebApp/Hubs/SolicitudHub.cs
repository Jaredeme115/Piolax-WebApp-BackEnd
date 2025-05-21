using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Piolax_WebApp.Hubs
{
    public class SolicitudHub : Hub
    {
        private static readonly object _connectionLock = new object();
        private static readonly Dictionary<string, int> _connectionAreas = new Dictionary<string, int>();

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"Solicitud Hub: Nueva conexión establecida: {connectionId}");

            // Si el usuario está autenticado, unirlo automáticamente a sus grupos de área
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var idAreas = Context.User.FindAll("idArea").Select(c => c.Value).ToList();
                foreach (var areaStr in idAreas)
                {
                    if (int.TryParse(areaStr, out int idArea))
                    {
                        await UnirseGrupoArea(idArea);
                    }
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"Solicitud Hub: Conexión terminada: {connectionId}. Motivo: {exception?.Message ?? "Desconexión normal"}");

            // Limpiar datos de conexión
            lock (_connectionLock)
            {
                if (_connectionAreas.ContainsKey(connectionId))
                {
                    _connectionAreas.Remove(connectionId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task NotificarActualizacionSolicitudes(int idArea, List<int> idRoles)
        {
            try
            {
                Console.WriteLine($"Solicitud Hub: Notificando actualización para área {idArea}");

                // Enviar a todos los clientes (no requiere membresía de grupo)
                await Clients.All.SendAsync("RecibirActualizacionSolicitudes", idArea, idRoles);

                // También enviar al grupo específico de área
                await Clients.Group($"Area_{idArea}").SendAsync("RecibirActualizacionSolicitudes", idArea, idRoles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en NotificarActualizacionSolicitudes: {ex.Message}");
            }
        }

        public async Task UnirseGrupoArea(int idArea)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                Console.WriteLine($"Solicitud Hub: Usuario {connectionId} uniéndose al grupo Area_{idArea}");

                // Almacenar el área asociada con esta conexión
                lock (_connectionLock)
                {
                    _connectionAreas[connectionId] = idArea;
                }

                await Groups.AddToGroupAsync(connectionId, $"Area_{idArea}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UnirseGrupoArea: {ex.Message}");
                throw;
            }
        }

        public async Task SalirGrupoArea(int idArea)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                Console.WriteLine($"Solicitud Hub: Usuario {connectionId} saliendo del grupo Area_{idArea}");

                // Eliminar el área asociada con esta conexión
                lock (_connectionLock)
                {
                    if (_connectionAreas.ContainsKey(connectionId))
                    {
                        _connectionAreas.Remove(connectionId);
                    }
                }

                await Groups.RemoveFromGroupAsync(connectionId, $"Area_{idArea}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SalirGrupoArea: {ex.Message}");
                throw;
            }
        }
    }
}
