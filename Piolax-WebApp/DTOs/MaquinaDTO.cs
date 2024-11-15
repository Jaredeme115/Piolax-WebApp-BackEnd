using Piolax_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class MaquinaDTO
    {
        [Required(ErrorMessage = "El nombre de la maquina es requerido")]
        [StringLength(30)]
        public string descripcion { get; set; }


        [Required]
        [StringLength(255)]
        public string codigoQR { get; set; }

        [Required]
        public int idArea { get; set; }


    }
}
