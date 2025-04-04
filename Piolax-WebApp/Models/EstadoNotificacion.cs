using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class EstadoNotificacion
    {
        [Key]
        public int IdEstadoNotificacion { get; set; }
        public string Descripcion { get; set; }

        public ICollection<Notificacion> Notificaciones { get; set; }
    }
}
