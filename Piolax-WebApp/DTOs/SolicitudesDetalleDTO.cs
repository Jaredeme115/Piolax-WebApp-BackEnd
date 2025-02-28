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

        //Nombre del Area y Rol asignado a la Solicitud
        public string area { get; set; }
        public string rol { get; set; }

        [Required]
        public int idCategoriaTicket { get; set; }

        //Asignar nombre a la maquina, al turno, al status de la orden, al status de aprobación del solicitante y a la categoria del ticket
        public string nombreMaquina { get; set; }
        public string nombreTurno { get; set; }
        public string nombreStatusOrden { get; set; }
        public string nombreStatusAprobacionSolicitante { get; set; }

        public string nombreCategoriaTicket { get; set; }

        //Atributo para ver el nombre completo tecnico
        public string nombreCompletoTecnico { get; set; }

    }
}
