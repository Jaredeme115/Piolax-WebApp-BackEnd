using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Piolax_WebApp.Models
{
    public class Solicitudes
    {
        [Key]
        public int idSolicitud { get; set; }

        [Required]
        public string descripcion { get; set; }

        [Required]
        public DateTime fechaSolicitud { get; set; }

        [Required]
        public int idEmpleado { get; set; }
        public Empleado Empleado { get; set; }

        [Required]
        public int idMaquina { get; set; }
        public Maquinas Maquina { get; set; }

        [Required]
        public int idTurno { get; set; }
        public Turnos Turno { get; set; }

        [Required]
        public int idStatusOrden { get; set; }
        public StatusOrden StatusOrden { get; set; }

        [Required]
        public int idStatusAprobacionSolicitante { get; set; }
        public StatusAprobacionSolicitante StatusAprobacionSolicitante { get; set; }

    }
}
