using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class asignacion_refacciones
    {
        [Key]
        public int idAsignacionRefaccion { get; set; }

        [Required]
        public int idAsignacion { get; set; }

        public Asignaciones Asignaciones { get; set; }

        [Required]
        public int idRefaccion { get; set; }

        public Inventario Inventario { get; set; }

        [Required]
        public int cantidad { get; set; }
    }
}
