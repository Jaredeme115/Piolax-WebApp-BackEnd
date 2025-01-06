using Piolax_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class InventarioDTO
    {
        [Required]
        public string descripcion { get; set; }

        [Required]
        public string ubicacion { get; set; }

        [Required]
        public string categoria { get; set; }

        [Required]
        public int cantidadActual { get; set; }

        [Required]
        public int cantidadMax { get; set; }

        [Required]
        public int cantidadMin { get; set; }

        [Required]
        public bool piezaCritica { get; set; }

        [Required]
        public string nombreProducto { get; set; }

        [Required]
        public string numParte { get; set; }

        [Required]
        public string proveedor { get; set; }

        [Required]
        public float precioUnitario { get; set; }

        [Required]
        public float precioInventarioTotal { get; set; }

        [Required]
        public string codigoBarra { get; set; }

        [Required]
        public string codigoQR { get; set; }

        [Required]
        public string proceso { get; set; }

        [Required]
        public int idArea { get; set; }

        [Required]
        public int idMaquina { get; set; }


        [Required]
        public DateTime fechaEntrega { get; set; }

        [Required]
        public bool inventarioActivoObsoleto { get; set; }
    }
}
