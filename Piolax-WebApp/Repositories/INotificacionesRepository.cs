using Piolax_WebApp.Models;

namespace Piolax_WebApp.Repositories
{
    public interface INotificacionesRepository
    {
        Task<IEnumerable<Notificacion>> ObtenerNotificaciones(int idEmpleado, string rol);
        Task<bool> MarcarComoLeida(int idNotificacion);
        Task<Notificacion> CrearNotificacion(Notificacion notificacion);
    }
}
