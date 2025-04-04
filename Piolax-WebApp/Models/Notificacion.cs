using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Notificacion
    {
        [Key] // 👈 ESTA ES LA CLAVE PRIMARIA
        public int idNotificacion { get; set; }
        public string titulo { get; set; }
        public string mensaje { get; set; }
        public DateTime fechaEnvio { get; set; }
        public bool leido { get; set; }
        public int idEmpleado { get; set; }
        public Empleado Empleado { get; set; }
        public string tipoNotificacion { get; set; }
    }
}
