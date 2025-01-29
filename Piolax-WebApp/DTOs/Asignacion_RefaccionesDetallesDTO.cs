using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class Asignacion_RefaccionesDetallesDTO
    {

        [Required]
        public int idAsignacionRefaccion { get; set; }

        [Required]
        public int idAsignacion { get; set; }

        [Required]
        public int idRefaccion { get; set; }

        public string? nombreRefaccion { get; set; } // En lugar del modelo completo

        [Required]
        public int idAsignacionTecnico { get; set; }

        [Required]
        public int cantidad { get; set; }
    }
}
