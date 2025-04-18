using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Notificacion
    {
        [Key]
        public int idNotificacion { get; set; }
        public int? idSolicitud { get; set; }
        public Solicitudes solicitud { get; set; }
        public int? idEmpleadoDestino { get; set; }
        public Empleado empleadoDestino { get; set; }
        public DateTime fechaEnvio { get; set; }
        public int idEstadoNotificacion { get; set; } = 1; // 1 = No leída
        public string tipoNotificacion { get; set; }
        public string mensaje { get; set; }
        public string rolDestino { get; set; }
        public string enlace { get; set; }
    }

    }
