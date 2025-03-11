using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Maquinas
    {
        [Key]
        public int idMaquina { get; set; }

        [Required]
        [StringLength(30)]
        public string nombreMaquina { get; set; }


        [Required]
        [StringLength(255)]
        public string codigoQR { get; set; }

        [Required]
        public int idArea { get; set; }
        public Areas Area { get; set; }

        public ICollection<Solicitudes> Solicitudes { get; set; } // Lista de solicitudes asociados a esta maquina

        public ICollection<Inventario> Inventario { get; set; } // Lista de inventario asociados a esta maquina

        public virtual ICollection<KpisMantenimiento> KpisMantenimientos { get; set; } = new List<KpisMantenimiento>(); // Lista de KPI´s de Mantenimiento asociados a Maquina

        public virtual ICollection<MantenimientoPreventivo> MantenimientosPreventivos { get; set; } = new List<MantenimientoPreventivo>(); // Lista de MAntenimientos Preventivos asociados a FrecuenciaMP


    }
}
