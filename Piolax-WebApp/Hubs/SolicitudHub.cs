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
        private readonly ILogger<SolicitudHub> _logger;

        public SolicitudHub(ILogger<SolicitudHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User?.Identity?.IsAuthenticated != true)
                throw new HubException("Usuario no autenticado");

            // Obtener el identificador de usuario (string)
            var userId = Context.UserIdentifier;
            // Obtener todas las áreas del claim "idArea"
            var areaClaims = Context.User
                                    .FindAll("idArea")
                                    .Select(c => c.Value)
                                    .Distinct();

            if (!areaClaims.Any())
                throw new HubException("Usuario sin áreas asignadas");

            // Construir lista de grupos a los que unirse
            var groups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 1) Grupo individual del usuario
            groups.Add($"User_{userId}");

            // 2) Grupo por cada área
            foreach (var area in areaClaims)
            {
                groups.Add($"Area_{area}");
            }

            _logger.LogInformation(
                "SolicitudHub: Conexión {ConnectionId} uniéndose a grupos: {Groups}",
                Context.ConnectionId,
                string.Join(", ", groups)
            );

            // Añadir a todos los grupos en paralelo
            await Task.WhenAll(groups
                .Select(g => Groups.AddToGroupAsync(Context.ConnectionId, g)));

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation(
                "SolicitudHub: Conexión {ConnectionId} desconectada (motivo: {Reason})",
                Context.ConnectionId,
                exception?.Message ?? "Normal"
            );
            // SignalR elimina automáticamente de los grupos
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Notifica a todos los miembros del área y al usuario creador
        /// que las solicitudes se han actualizado.
        /// </summary>
        /// <param name="idArea">ID del área</param>
        /// <param name="idRoles">Lista de roles que pueden estar interesados</param>
        /// <param name="empleadoSolicitanteId">ID del usuario que generó la solicitud</param>
        public Task NotificarActualizacionSolicitudes(
            int idArea,
            int[] idRoles,
            string empleadoSolicitanteId)
        {
            var payload = new
            {
                idArea,
                idRoles
            };

            _logger.LogInformation(
                "NotificarActualizacionSolicitudes: área={Area}, roles=[{Roles}], creador={User}",
                idArea,
                string.Join(",", idRoles),
                empleadoSolicitanteId
            );

            // Enviar al grupo de área
            var tArea = Clients.Group($"Area_{idArea}")
                               .SendAsync("RecibirActualizacionSolicitudes", payload);

            // Enviar al grupo individual del creador
            var tUser = Clients.Group($"User_{empleadoSolicitanteId}")
                               .SendAsync("RecibirActualizacionSolicitudes", payload);

            return Task.WhenAll(tArea, tUser);
        }

        /// <summary>
        /// Permite al cliente unirse dinámicamente a su grupo de área.
        /// </summary>
        public Task JoinAreaGroup(int idArea)
        {
            var groupName = $"Area_{idArea}";
            _logger.LogInformation(
                "SolicitudHub: {ConnectionId} se une al grupo {Group}",
                Context.ConnectionId,
                groupName
            );
            return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Permite al cliente salir dinámicamente de su grupo de área.
        /// </summary>
        public Task LeaveAreaGroup(int idArea)
        {
            var groupName = $"Area_{idArea}";
            _logger.LogInformation(
                "SolicitudHub: {ConnectionId} sale del grupo {Group}",
                Context.ConnectionId,
                groupName
            );
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}