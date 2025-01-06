using Microsoft.AspNetCore.Routing.Constraints;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Inventario
    {
        [Key]
        public int idRefaccion { get; set; }

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

        public Areas Areas { get; set; }

        [Required]
        public int idMaquina { get; set; }

        public Maquinas Maquinas { get; set; }

        [Required]
        public DateTime fechaEntrega { get; set; }

        [Required]
        public bool inventarioActivoObsoleto { get; set; }


        public virtual ICollection<Asignaciones> Asignaciones { get; set; } = new List<Asignaciones>(); // Lista de Asignaciones asociados a Inventario

        public virtual ICollection<asignacion_refacciones> Asignacion_Refacciones { get; set; } = new List<asignacion_refacciones>(); // Lista de asignacion_refacciones asociados a Inventario
    }
}
