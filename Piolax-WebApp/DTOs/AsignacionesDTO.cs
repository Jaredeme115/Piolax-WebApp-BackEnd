using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class AsignacionesDTO
    {
        [Required]
        public int idSolicitud { get; set; }

        [Required]
        public int idEmpleado { get; set; }

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
    }
}
