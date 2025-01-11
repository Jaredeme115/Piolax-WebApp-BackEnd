using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class AsignacionesDetalleDTO
    {
        [Required]
        public int idAsignacion { get; set; }

        [Required]
        public int idSolicitud { get; set; }

        [Required]
        public string nombreCompletoTecnico { get; set; }

        [Required]
        public bool qrScaneado { get; set; }

        [Required]
        public TimeSpan horaInicio { get; set; }

        [Required]
        public TimeSpan horaTermino { get; set; }

        [Required]
        public string solucion { get; set; }

        [Required]
        public int idRefaccion { get; set; }

        [Required]
        public int cantidad { get; set; }

        [Required]
        public bool maquinaDetenida { get; set; }

        [Required]
        public int idCategoriaAsignacion { get; set; }

        [Required]
        public int idStatusAprobacionTecnico { get; set; }

        //Asignar nombre a la refaccion de la asignacion
        public string nombreRefaccion { get; set; }

        //Asignar nombre a la categoria de la asignacion
        public string nombreCategoriaAsignacion { get; set; }

        //Asignar nombre al status de aprobación del tecnico
        public string nombreStatusAprobacionTecnico { get; set; }
    }
}
