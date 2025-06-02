using Piolax_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class Asignacion_TecnicoFinalizacionDTO
    {
        [Required]
        public int idAsignacion { get; set; }

        [Required]
        public int idEmpleado { get; set; }

        [Required]
        public string solucion { get; set; }

        public bool paroMaquinaTecnico { get; set; }

    }
}
