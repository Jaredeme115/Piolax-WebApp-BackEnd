using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class InventarioCategoriasDTO
    {
        [Required]
        public string nombreInventarioCategoria { get; set; }
    }
}
