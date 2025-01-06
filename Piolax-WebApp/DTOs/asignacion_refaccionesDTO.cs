using Piolax_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class asignacion_refaccionesDTO
    {
        [Required]
        public int idAsignacion { get; set; }

        [Required]
        public int idRefaccion { get; set; }

        [Required]
        public int cantidad { get; set; }
    }
}
