using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Asignaciones
    {
        [Key]
        public int idAsignacion { get; set; }

        [Required]
        public int idSolicitud { get; set; }

        [Required]
        public int idEmpleado { get; set; }

        [Required]
        public bool qrScaneado { get; set; }

        [Required]
        public DateTime horaInicio { get; set; }

        [Required]
        public DateTime horaTermino { get; set; }

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


    }
}
