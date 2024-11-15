using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class StatusAprobacionSolicitanteDTO
    {
        [Required]
        [StringLength(30)]
        public string descripcionStatusAprobacionSolicitante { get; set; }
    }
}
