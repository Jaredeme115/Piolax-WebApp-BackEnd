using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class AreaDTO
    {
        [Required(ErrorMessage = "El nombre del area es requerido")]
        public string nombreArea { get; set; }

        public int contadorMaquinasActivasPorArea { get; set; }
    }
}
