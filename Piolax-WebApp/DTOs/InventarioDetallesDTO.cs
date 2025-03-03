using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class InventarioDetalleDTO
    {
        public int idRefaccion { get; set; }

        [Required]
        public string descripcion { get; set; }

        [Required]
        public string ubicacion { get; set; }

        [Required]
        public int idInventarioCategoria { get; set; }

        public string nombreInventarioCategoria { get; set; } // 🔹 Nombre de la categoría

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
        public string codigoQR { get; set; }

        [Required]
        public string proceso { get; set; }

        [Required]
        public int idArea { get; set; }

        public string nombreArea { get; set; } // 🔹 Nombre del área asignada

        [Required]
        public int idMaquina { get; set; }

        public string nombreMaquina { get; set; } // 🔹 Nombre de la máquina asignada

        [Required]
        public DateTime fechaEntrega { get; set; }

        [Required]
        public bool inventarioActivoObsoleto { get; set; }

        [Required]
        public string item { get; set; }

        [Required]
        public DateTime fechaActualizacion { get; set; }

        [Required]
        public string EstatusInventario { get; set; }
    }

}
