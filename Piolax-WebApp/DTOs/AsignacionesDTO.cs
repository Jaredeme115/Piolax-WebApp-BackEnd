using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class AsignacionesDTO
    {
        [Required]
        public int idSolicitud { get; set; }

        [Required]
        public int idStatusAsignacion { get; set; }

    }
}
