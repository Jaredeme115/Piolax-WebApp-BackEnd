using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class StatusAprobacionTecnicoDTO
    {
        [Required]
        [StringLength(30)]
        public string descripcionStatusAprobacionTecnico { get; set; }
    }
}
 