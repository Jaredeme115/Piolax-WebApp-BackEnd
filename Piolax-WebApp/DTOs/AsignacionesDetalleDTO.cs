using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class AsignacionesDetalleDTO
    {
        [Required]
        public int idAsignacion { get; set; }

        [Required]
        public string descripcion { get; set; }

        [Required]
        public DateTime fechaAsignacion { get; set; }

        [Required]
        public string nombreCompletoTecnico { get; set; }

        [Required]
        public int idMaquina { get; set; }

        [Required]
        public int idTurno { get; set; }

        [Required]
        public int idStatusOrden { get; set; }

        [Required]
        public int idStatusAprobacionTecnico { get; set; }

        //Nombre del Area y Rol asignado a la Solicitud
        public string area { get; set; }
        public string rol { get; set; }

        [Required]
        public string paroMaquina { get; set; }

        //Asignar nombre a la maquina, al turno, al status de la orden y al status de aprobación del solicitante
        public string nombreMaquina { get; set; }
        public string nombreTurno { get; set; }
        public string nombreStatusOrden { get; set; }
        public string nombreStatusAprobacionTecnico { get; set; }
    }
}
