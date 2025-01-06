using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Asignaciones
    {
        [Key]
        public int idAsignacion { get; set; }

        [Required]
        public int idSolicitud { get; set; }

        public Solicitudes Solicitud { get; set; }

        [Required]
        public int idEmpleado { get; set; }

        public Empleado Empleado { get; set; }

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

        public Inventario Inventario { get; set; }

        [Required]
        public int cantidad { get; set; }

        [Required]
        public bool maquinaDetenida { get; set; }

        [Required]
        public int idCategoriaAsignacion { get; set; }

        public CategoriaAsignacion CategoriaAsignacion { get; set; }

        [Required]
        public int idStatusAprobacionTecnico { get; set; }

        public StatusAprobacionTecnico StatusAprobacionTecnico { get; set; }

        public virtual ICollection<asignacion_refacciones> Asignacion_Refacciones { get; set; } = new List<asignacion_refacciones>(); // Lista de asignacion_refacciones asociados a Asignaciones

    }
}
