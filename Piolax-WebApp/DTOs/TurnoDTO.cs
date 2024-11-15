using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class TurnoDTO
    {
        [Required]
        [StringLength(30)]
        public string descripcion { get; set; }

        [Required]
        public TimeOnly horaInicio { get; set; }

        [Required]
        public TimeOnly horaFinal { get; set; }
    }
}
