using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class InventarioModificarDTO
    {
        [Required]
        public string descripcion { get; set; }

        [Required]
        public string ubicacion { get; set; }

        [Required]
        public int cantidadActual { get; set; }

        [Required]
        public int cantidadMax { get; set; }

        [Required]
        public int cantidadMin { get; set; }

        [Required]
        public bool piezaCritica { get; set; }

        [Required]
        public string proveedor { get; set; }

        [Required]
        public float precioUnitario { get; set; }

        [Required]
        public string proceso { get; set; }

        [Required]
        public int idMaquina { get; set; } // 🔹 Se selecciona de una lista filtrada por área

        [Required]
        public DateTime fechaEntrega { get; set; }

        [Required]
        public bool inventarioActivoObsoleto { get; set; }
    }
}
