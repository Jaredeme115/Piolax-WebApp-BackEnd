using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class SolicitudesDetalleDTO
    {
        [Required]
        public int idSolicitud { get; set; }
        [Required]
        public string descripcion { get; set; }
        [Required]
        public DateTime fechaSolicitud { get; set; }
        [Required]
        public string nombreCompletoEmpleado { get; set; }

        [Required]
        public int idMaquina { get; set; }

        [Required]
        public int idTurno { get; set; }

        [Required]
        public int idStatusOrden { get; set; }

        [Required]
        public int idStatusAprobacionSolicitante { get; set; }


        public List<string> Areas { get; set; }
        public List<string> Roles { get; set; }

        [Required]
        public string paroMaquina { get; set; }
    }
}
