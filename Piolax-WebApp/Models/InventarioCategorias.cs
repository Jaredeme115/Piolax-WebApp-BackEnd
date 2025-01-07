using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class InventarioCategorias
    {
        [Key]
        public int idInventarioCategoria { get; set; }

        [Required]
        public string nombreInventarioCategoria { get; set; }

        public ICollection<Inventario> Inventario { get; set; } // Lista de inventario asociados a esta categoria
    }
}
