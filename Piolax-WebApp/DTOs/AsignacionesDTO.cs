using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class AsignacionesDTO
    {
        [Required]
        public int idSolicitud { get; set; }

        [Required]
        public int idStatusAsignacion { get; set; }

        // Campo Temporal para el QR
        [Required(ErrorMessage = "El código QR es requerido.")]
        public string codigoQR { get; set; }

    }
}
