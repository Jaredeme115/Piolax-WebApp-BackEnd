using Piolax_WebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Piolax_WebApp.DTOs
{
    public class Asignacion_TecnicoDTO
    {
        [Required]
        public int idAsignacion { get; set; }
       
        [Required]
        public int idEmpleado { get; set; }

        [Required]
        public DateTime horaInicio { get; set; }

        [Required]
        public DateTime horaTermino { get; set; }

        [Required]
        public string solucion { get; set; }

        [Required]
        public int idStatusAprobacionTecnico { get; set; }

        [Required]
        public string comentarioPausa { get; set; }

        [Required]
        public bool esTecnicoActivo { get; set; }

        // Campo temporal: No se guarda en la base de datos
        [Required]
        public string codigoQR { get; set; } // Código QR escaneado por el técnico
    }
}
