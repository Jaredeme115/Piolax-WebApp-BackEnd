using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class Asignacion_TecnicoDetallesDTO
    {
        [Required]
        public int idAsignacionTecnico { get; set; }

        [Required]
        public int idAsignacion { get; set; }

        [Required]
        public int idEmpleado { get; set; }

        public string? nombreCompletoTecnico { get; set; } // En lugar del modelo completo

        [Required]
        public DateTime horaInicio { get; set; }

        [Required]
        public DateTime horaTermino { get; set; }

        [Required]
        public string solucion { get; set; }

        [Required]
        public int idStatusAprobacionTecnico { get; set; }
        public string nombreStatusAprobacionTecnico { get; set; }

        [Required]
        public string comentarioPausa { get; set; }

        [Required]
        public bool esTecnicoActivo { get; set; }



        // Refacciones relacionados con el Tecnico
        public List<Asignacion_RefaccionesDetallesDTO> Refacciones { get; set; } = new List<Asignacion_RefaccionesDetallesDTO>();

    }
}
