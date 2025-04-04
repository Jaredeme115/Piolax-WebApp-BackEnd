using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Services
{
    public interface INotificacionService
    {
        Task CrearNotificacion(NotificacionDTO dto);
        Task CrearNotificacionPorRol(string titulo, string mensaje, string tipoNotificacion, int idRol);
        Task<List<Notificacion>> ObtenerNotificacionesPorUsuario(int idEmpleado);
        Task MarcarComoLeida(int id);



    }
}
