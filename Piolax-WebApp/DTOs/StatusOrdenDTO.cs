using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class StatusOrdenDTO
    {
        [Required]
        [StringLength(30)]
        public string descripcionStatusOrden { get; set; }

        [Required]
        [StringLength(15)]
        public string color { get; set; }
    }
}
