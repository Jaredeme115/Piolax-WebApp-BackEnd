using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class SolicitudesDetalleCompletoDTO
    {
        // 📌 Datos generales de la solicitud
        [Required] public int idSolicitud { get; set; }
        [Required] public string descripcion { get; set; }
        [Required] public DateTime fechaSolicitud { get; set; }
        [Required] public string nombreCompletoEmpleado { get; set; }
        [Required] public int idMaquina { get; set; }
        [Required] public int idTurno { get; set; }
        [Required] public int idStatusOrden { get; set; }
        [Required] public int idStatusAprobacionSolicitante { get; set; }
        public string area { get; set; }
        public string rol { get; set; }
        [Required] public int idCategoriaTicket { get; set; }
        public string nombreMaquina { get; set; }
        public string nombreTurno { get; set; }
        public string nombreStatusOrden { get; set; }
        public string nombreStatusAprobacionSolicitante { get; set; }
        public string nombreCategoriaTicket { get; set; }
        public bool paroMaquina { get; set; }

        // 📌 Información sobre técnicos asignados
        public List<Asignacion_TecnicoDetallesDTO> tecnicos { get; set; } = new();

        // 📌 Refacciones utilizadas
        public List<Asignacion_RefaccionesDetallesDTO> refacciones { get; set; } = new();

        // 📌 Información de tiempos
        public DateTime? fechaHoraInicio { get; set; }
        public DateTime? fechaHoraFin { get; set; }
        public double tiempoTotalTrabajo { get; set; } = 0; // En minutos
        public double tiempoEspera { get; set; } = 0; // Tiempo total de espera antes de ser tomada
    }

}
